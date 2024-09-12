# Contributing

This document details how to contribute to the PowerPlaywright project. Please ensure you have read all of this document before contributing.

## Table of Contents

- [Contributing](#contributing)
  - [Table of Contents](#table-of-contents)
  - [Issues](#issues)
  - [Overview](#overview)
    - [PowerPlaywright](#powerplaywright)
    - [PowerPlaywright.Framework](#powerplaywrightframework)
    - [PowerPlaywright.Strategies](#powerplaywrightstrategies)
  - [Branching strategy](#branching-strategy)
  - [Versioning](#versioning)
  - [Testing](#testing)
  - [Pull requests](#pull-requests)

## Issues

Please first discuss the change you wish to make via an issue before making a change.

## Overview

PowerPlaywright is comprised of three separate libraries.

### PowerPlaywright

The internal implementation of the public API described in **PowerPlaywright.Framework** (except for control classes) along with any other internal classes.

### PowerPlaywright.Framework

The interfaces that describe the API as well as any concrete implemenations required by both **PowerPlaywright** and **PowerPlaywright.Strategies**.

### PowerPlaywright.Strategies

This library is dependent on the interfaces described in **PowerPlaywright.Framework** and is dynamically fetched and loaded at runtime.

Anything that might be impacted by platform updates (e.g., controls and redirectors) will be implemented here. This ensures that tests can be kept in sync with changes in the platform.

## Branching strategy

We are using a [GitHub Flow](https://githubflow.github.io/) workflow. In short:

- Create a topic branch from master
- Implement changes on your topic branch
- Create a pull request into master
- Address review comments or validation pipeline issues

A new package version will be published when the pull request is merged. 

## Versioning

We use [GitVersion](https://gitversion.net/docs/) to automatically version the NuGet packages within this project based on commit messages. 

GitVersion has been configured to version by commit messages based on the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification. Please therefore ensure that you are writing commit messages according to this spec.

## Testing

All changes should be covered by automated unit and integration tests. Please ensure that you either update existing tests or write additional tests if required.

Refer to the [README.md](./tests/PowerPlaywright.IntegrationTests/README.md) within the integration test project for information on how to run these tests.

## Pull requests

A maintainer will merge your pull request once it meets all of the required checks. Please ensure that you:

- Update the README.md with details of any new or updated functionality
- Create or update unit and integration tests