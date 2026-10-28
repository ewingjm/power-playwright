namespace PowerPlaywright.IntegrationTests.Extensions;

using Microsoft.Playwright;
using PowerPlaywright.Framework.Extensions;

/// <summary>
/// Tests for <see cref="IPageExtensions"/> using a controlled browser page.
/// </summary>
public class IPageExtensionsTests : IntegrationTests
{
    private IPage page = null!;

    /// <summary>
    /// Creates a page with a controllable app idle tracker.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [SetUp]
    public async Task SetUpPageAsync()
    {
        this.page = await this.Context.NewPageAsync();
        await this.page.SetContentAsync("<main>Test app</main>");
        await this.page.EvaluateAsync(@"() => {
            window.idleObservers = new Set();
            const Observer = window.MutationObserver;
            window.MutationObserver = class extends Observer {
                observe(...args) {
                    super.observe(...args);
                    window.idleObservers.add(this);
                }
                disconnect() {
                    super.disconnect();
                    window.idleObservers.delete(this);
                }
            };
            window.idleReadings = [];
            window.UCWorkBlockTracker = {
                isAppIdle() {
                    window.idleReadings.push(performance.now());
                    return true;
                }
            };
        }");
    }

    /// <summary>
    /// Verifies that observers are disconnected after success, timeout, or error.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TearDown]
    public async Task VerifyObserverCleanupAsync()
    {
        Assert.That(await this.page.EvaluateAsync<int>("window.idleObservers.size"), Is.Zero);
    }

    /// <summary>
    /// Requires at least seven idle readings spanning the quiet period.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WaitForAppIdleAsync_AlreadyIdle_SamplesForEntireQuietPeriod()
    {
        await this.page.WaitForAppIdleAsync();

        var readings = await this.page.EvaluateAsync<double[]>("window.idleReadings");
        Assert.That(readings, Has.Length.GreaterThanOrEqualTo(7));
        Assert.That(readings[^1] - readings[0], Is.GreaterThanOrEqualTo(300));
    }

    /// <summary>
    /// Restarts the shared quiet period for DOM mutations and non-idle tracker readings.
    /// </summary>
    /// <param name="activity">The activity interrupting the quiet period.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TestCase("busy")]
    [TestCase("missingTracker")]
    [TestCase("missingFunction")]
    [TestCase("undefined")]
    [TestCase("childList")]
    [TestCase("attributes")]
    [TestCase("characterData")]
    [TestCase("finalSampleMutation")]
    public async Task WaitForAppIdleAsync_ActivityDuringQuietPeriod_RestartsQuietPeriod(string activity)
    {
        await this.page.EvaluateAsync(
            @"activity => {
            const tracker = window.UCWorkBlockTracker;
            tracker.isAppIdle = function() {
                window.idleReadings.push(performance.now());
                if (window.idleReadings.length === (activity === 'finalSampleMutation' ? 7 : 4)) {
                    window.lastActivity = performance.now();
                    switch (activity) {
                        case 'busy':
                            return false;
                        case 'missingTracker':
                            delete window.UCWorkBlockTracker;
                            setTimeout(() => { window.UCWorkBlockTracker = tracker; }, 100);
                            return false;
                        case 'missingFunction':
                            const sample = tracker.isAppIdle;
                            delete tracker.isAppIdle;
                            setTimeout(() => { tracker.isAppIdle = sample; }, 100);
                            return false;
                        case 'undefined':
                            return undefined;
                        case 'childList':
                            document.querySelector('main').appendChild(document.createElement('span'));
                            break;
                        case 'attributes':
                        case 'finalSampleMutation':
                            document.querySelector('main').setAttribute('data-test', 'changed');
                            break;
                        case 'characterData':
                            document.querySelector('main').firstChild.data = 'Changed';
                            break;
                    }
                }
                return true;
            };
        }",
            activity);

        await this.page.WaitForAppIdleAsync();

        var elapsed = await this.page.EvaluateAsync<double>("performance.now() - window.lastActivity");
        var readingsAfterActivity = await this.page.EvaluateAsync<double[]>("window.idleReadings.filter(time => time > window.lastActivity)");
        Assert.That(elapsed, Is.GreaterThanOrEqualTo(300));
        Assert.That(readingsAfterActivity, Has.Length.GreaterThanOrEqualTo(7));
        Assert.That(readingsAfterActivity[^1] - readingsAfterActivity[0], Is.GreaterThanOrEqualTo(300));
    }

    /// <summary>
    /// Requires a single quiet period after the last of both DOM and tracker activity.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WaitForAppIdleAsync_DomAndTrackerActivityOverlap_RequiresSharedQuietPeriod()
    {
        await this.page.EvaluateAsync(@"() => {
            window.UCWorkBlockTracker.isAppIdle = () => {
                window.idleReadings.push(performance.now());
                if (window.idleReadings.length === 4) {
                    document.querySelector('main').setAttribute('data-test', 'changed');
                }
                if (window.idleReadings.length === 8) {
                    window.lastBusy = performance.now();
                    return false;
                }
                return true;
            };
        }");

        await this.page.WaitForAppIdleAsync();

        var readings = await this.page.EvaluateAsync<double[]>("window.idleReadings.filter(time => time > window.lastBusy)");
        Assert.That(readings, Has.Length.GreaterThanOrEqualTo(7));
        Assert.That(readings[^1] - readings[0], Is.GreaterThanOrEqualTo(300));
    }

    /// <summary>
    /// Bounds the wait when the DOM keeps changing even though the tracker is idle.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WaitForAppIdleAsync_ContinuousDomMutations_ContinuesAfterTimeout()
    {
        await this.page.EvaluateAsync(@"() => {
            window.mutationTimer = setInterval(() => {
                document.querySelector('main').setAttribute('data-test', performance.now());
            }, 25);
        }");
        var started = await this.page.EvaluateAsync<double>("performance.now()");

        try
        {
            await this.page.WaitForAppIdleAsync(TimeSpan.FromMilliseconds(600));

            var elapsed = await this.page.EvaluateAsync<double>("performance.now()") - started;
            Assert.That(elapsed, Is.GreaterThanOrEqualTo(600).And.LessThan(2000));
        }
        finally
        {
            await this.page.EvaluateAsync("clearInterval(window.mutationTimer)");
        }
    }

    /// <summary>
    /// Uses one bounded timeout even when the tracker stays busy or is unavailable.
    /// </summary>
    /// <param name="tracker">The tracker configuration.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TestCase("{ isAppIdle: () => false }")]
    [TestCase("undefined")]
    [TestCase("{}")]
    public async Task WaitForAppIdleAsync_NeverIdle_ContinuesAfterTimeout(string tracker)
    {
        await this.page.EvaluateAsync($"window.UCWorkBlockTracker = {tracker}");
        var started = await this.page.EvaluateAsync<double>("performance.now()");

        await this.page.WaitForAppIdleAsync(TimeSpan.FromMilliseconds(200));

        var elapsed = await this.page.EvaluateAsync<double>("performance.now()") - started;
        Assert.That(elapsed, Is.GreaterThanOrEqualTo(200).And.LessThan(2000));
    }

    /// <summary>
    /// Propagates tracker errors rather than treating them as successful idle checks.
    /// </summary>
    [Test]
    public void WaitForAppIdleAsync_TrackerThrows_PropagatesError()
    {
        Assert.That(
            async () =>
            {
                await this.page.EvaluateAsync("window.UCWorkBlockTracker.isAppIdle = () => { throw new Error('Tracker failed'); }");
                await this.page.WaitForAppIdleAsync();
            },
            Throws.TypeOf<PlaywrightException>().With.Message.Contains("Tracker failed"));
    }

    /// <summary>
    /// Still waits for the save indicator after the shared quiet period completes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WaitForAppIdleAsync_SaveInProgress_WaitsForSaveToFinish()
    {
        await this.page.EvaluateAsync(@"() => {
            const saving = document.createElement('div');
            saving.setAttribute('role', 'alert');
            saving.textContent = 'Saving...';
            document.body.appendChild(saving);
            setTimeout(() => {
                saving.remove();
                window.saved = true;
            }, 1000);
        }");

        await this.page.WaitForAppIdleAsync();

        Assert.That(await this.page.EvaluateAsync<bool>("window.saved === true"), Is.True);
    }
}
