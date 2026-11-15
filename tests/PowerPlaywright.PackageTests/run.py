#!/usr/bin/env python3
"""Test real packages and NuGet restore policy using an isolated local feed."""

import argparse
import http.server
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import threading
import xml.etree.ElementTree as ET
import zipfile


ROOT = Path(__file__).resolve().parents[2]
TEMPLATE = Path(__file__).resolve().parent / "Consumer"


def run(*args, cwd=ROOT, env=None):
    result = subprocess.run(args, cwd=cwd, env=env, text=True, stdout=subprocess.PIPE,
                            stderr=subprocess.STDOUT, timeout=300)
    if result.returncode:
        raise RuntimeError(f"{' '.join(map(str, args))}\n{result.stdout}")
    return result.stdout


def pack(source, feed, version, additive):
    # Keep assembly identities stable within a major release, so an accidental
    # mixture would expose the missing-interface/type/member failure in #182.
    contract = """
namespace PowerPlaywright.Framework
{
    public static class PackageTestContract
    {
        public static string GetRelease() => "1.0.0";
    }
}
"""
    implementation = """
namespace PowerPlaywright.Strategies
{
    public class PackageTestStrategy
    {
        public string GetRelease() => Framework.PackageTestContract.GetRelease();
    }
}
"""
    if additive:
        contract += """
namespace PowerPlaywright.Framework
{
    public interface IPackageTestContract { string GetRelease(); }
    public static class PackageTestRelease
    {
        public static string Value => "VERSION";
    }
}
""".replace("VERSION", version)
        implementation = """
namespace PowerPlaywright.Strategies
{
    public class PackageTestStrategy : Framework.IPackageTestContract
    {
        public string GetRelease() => Framework.PackageTestRelease.Value;
    }
}
"""
    (source / "PowerPlaywright.Framework/PackageTestContract.cs").write_text(contract)
    (source / "PowerPlaywright.Strategies/PackageTestStrategy.cs").write_text(implementation)
    project = str(source / "PowerPlaywright/PowerPlaywright.csproj")
    properties = ["-p:DisableGitVersionTask=true", f"-p:Version={version}",
                  "-p:AssemblyVersion=1.0.0.0", f"-p:InformationalVersion={version}",
                  "-p:IncludeSourceRevisionInInformationalVersion=false"]
    run("dotnet", "build", project, "-c", "Release", *properties)
    run("dotnet", "pack", project, "-c", "Release", "--no-build", "--no-restore",
        "--output", str(feed), "-p:IncludeSymbols=true", "-p:SymbolPackageFormat=snupkg", *properties)
    with zipfile.ZipFile(feed / f"PowerPlaywright.{version}.nupkg") as package:
        names = package.namelist()
        for assembly in ("PowerPlaywright", "PowerPlaywright.Framework", "PowerPlaywright.Strategies"):
            assert f"lib/netstandard2.0/{assembly}.dll" in names, names
        nuspec = ET.fromstring(package.read(next(n for n in names if n.endswith(".nuspec"))))
        dependencies = [node.attrib["id"] for node in nuspec.iter()
                        if node.tag.endswith("}dependency")]
        assert not any(name.startswith(("PowerPlaywright", "NuGet.")) for name in dependencies), dependencies
    with zipfile.ZipFile(feed / f"PowerPlaywright.{version}.snupkg") as symbols:
        for assembly in ("PowerPlaywright", "PowerPlaywright.Framework", "PowerPlaywright.Strategies"):
            assert f"lib/netstandard2.0/{assembly}.pdb" in symbols.namelist()
    print(f"Packed coherent release {version}", flush=True)


def consumer(work, name, version, framework):
    path = work / name
    shutil.copytree(TEMPLATE, path, ignore=shutil.ignore_patterns("bin", "obj"))
    for project in path.rglob("*.csproj"):
        project.write_text(project.read_text().replace('Version="1.*"', f'Version="{version}"')
                           .replace("net8.0", framework))
    return path


def restore(path, *options):
    # .NET 8 calls this --no-cache; newer SDKs use --no-http-cache.
    cache_option = "--no-http-cache" if "--no-http-cache" in run("dotnet", "restore", "--help") else "--no-cache"
    run("dotnet", "restore", "Consumer.csproj", cache_option, "-p:NuGetAudit=false",
        *options, cwd=path)


def selected(path, expected):
    assets = json.loads((path / "obj/project.assets.json").read_text())
    packages = [name for name in assets["libraries"] if name.startswith("PowerPlaywright/")]
    assert packages == ["PowerPlaywright/" + expected], packages
    print(f"{path.name}: resolved {expected}", flush=True)


class RejectRequests(http.server.BaseHTTPRequestHandler):
    requests = []

    def reject(self):
        self.requests.append(self.path)
        self.send_error(502, "Network access disabled for package-consumer smoke test")

    do_CONNECT = reject
    do_GET = reject
    do_POST = reject

    def log_message(self, *args):
        pass


def execute(path, expected, framework, work, build=True):
    if build:
        run("dotnet", "build", "Consumer.csproj", "-c", "Release", "--no-restore", cwd=path)
    # No NuGet config, cache, credentials, or usable package-source connection
    # is provided to execution. Mocked Playwright calls need no browser/server.
    offline = work / "offline"
    offline.mkdir(exist_ok=True)
    (offline / "NuGet.Config").write_text("<configuration><packageSources><clear /></packageSources></configuration>")
    runtime_env = {key: os.environ[key] for key in (
        "PATH", "DOTNET_ROOT", "DOTNET_CLI_HOME", "TMPDIR", "TEMP", "TMP", "SystemRoot", "WINDIR")
        if key in os.environ}
    runtime_env["NUGET_PACKAGES"] = str(offline / "empty-cache")
    with http.server.ThreadingHTTPServer(("127.0.0.1", 0), RejectRequests) as proxy:
        thread = threading.Thread(target=proxy.serve_forever, daemon=True)
        thread.start()
        address = f"http://127.0.0.1:{proxy.server_port}"
        for key in ("HTTP_PROXY", "HTTPS_PROXY", "ALL_PROXY", "http_proxy", "https_proxy", "all_proxy"):
            runtime_env[key] = address
        runtime_env.update(NO_PROXY="", no_proxy="")
        try:
            output = run("dotnet", str(path / f"bin/Release/{framework}/Consumer.dll"), expected,
                         cwd=offline, env=runtime_env)
            assert not RejectRequests.requests, RejectRequests.requests
            print(output.strip(), flush=True)
        finally:
            proxy.shutdown()
            thread.join()


def test(work, framework):
    source = work / "src"
    shutil.copytree(ROOT / "src", source, ignore=shutil.ignore_patterns("bin", "obj"))
    feed = work / "feed"
    feed.mkdir()

    # Prime normal build/test dependencies using the repository's configured
    # sources. Consumer restores below then use ONLY this temporary local feed.
    run("dotnet", "restore", str(ROOT / "tests/PowerPlaywright.UnitTests/PowerPlaywright.UnitTests.csproj"))
    cache_output = run("dotnet", "nuget", "locals", "global-packages", "--list")
    cache = Path(cache_output.strip().split(": ", 1)[1])
    for package in cache.rglob("*.nupkg"):
        if not package.name.lower().startswith("powerplaywright"):
            try:
                os.link(package, feed / package.name)
            except OSError:
                shutil.copyfile(package, feed / package.name)
    (work / "NuGet.Config").write_text(
        f'<configuration><packageSources><clear /><add key="local" value="{feed}" />'
        '</packageSources></configuration>')
    # Use a clean cache for all consumer restores, separate from the bootstrap.
    previous_cache = os.environ.get("NUGET_PACKAGES")
    os.environ["NUGET_PACKAGES"] = str(work / "consumer-cache")
    try:
        pack(source, feed, "1.0.0", additive=False)
        floating = consumer(work, "floating", "1.*", framework)
        exact = consumer(work, "exact", "[1.0.0]", framework)
        locked = consumer(work, "locked", "1.*", framework)
        for path in (floating, exact, locked):
            restore(path, "--force-evaluate", "--use-lock-file")
            selected(path, "1.0.0")
            execute(path, "1.0.0", framework, work)

        pack(source, feed, "1.1.0", additive=True)
        pack(source, feed, "1.2.0-preview.1", additive=True)
        # Publishing a fix does not change a previously resolved/built suite.
        execute(floating, "1.0.0", framework, work, build=False)
        restore(floating, "--force-evaluate")
        selected(floating, "1.1.0")  # Stable 1.* excludes prereleases.
        execute(floating, "1.1.0", framework, work)
        restore(exact, "--force-evaluate")
        selected(exact, "1.0.0")
        execute(exact, "1.0.0", framework, work)
        lock_files = {path: path.read_bytes() for path in locked.rglob("packages.lock.json")}
        restore(locked, "--locked-mode")
        assert all(path.read_bytes() == contents for path, contents in lock_files.items())
        selected(locked, "1.0.0")
        execute(locked, "1.0.0", framework, work)
        restore(locked, "--force-evaluate", "--use-lock-file")
        selected(locked, "1.1.0")
        execute(locked, "1.1.0", framework, work)

        # An exact reference changes only when edited. Locked mode must reject
        # the edited inputs until the lock file is explicitly regenerated.
        for project in exact.rglob("*.csproj"):
            project.write_text(project.read_text().replace("[1.0.0]", "[1.1.0]"))
        try:
            restore(exact, "--locked-mode")
        except RuntimeError as error:
            assert "NU1004" in str(error), str(error)
        else:
            raise AssertionError("Locked restore accepted changed package inputs")
        restore(exact, "--force-evaluate")
        selected(exact, "1.1.0")
        execute(exact, "1.1.0", framework, work)
    finally:
        if previous_cache is None:
            os.environ.pop("NUGET_PACKAGES", None)
        else:
            os.environ["NUGET_PACKAGES"] = previous_cache


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--framework", default="net8.0", help="Consumer target framework")
    args = parser.parse_args()
    work = Path(tempfile.mkdtemp(prefix="power-playwright-packages-"))
    try:
        test(work, args.framework)
    except Exception:
        print(f"Package test files retained at {work}", flush=True)
        raise
    else:
        shutil.rmtree(work)
        print("Package-consumer regression passed", flush=True)
