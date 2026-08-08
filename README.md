# basketball-progress-tracker

[![Azure Static Web Apps CI/CD](https://github.com/nathanpee2006/basketball-progress-tracker/actions/workflows/azure-static-web-apps-lively-stone-0f9793c00.yml/badge.svg)](https://github.com/nathanpee2006/basketball-progress-tracker/actions/workflows/azure-static-web-apps-lively-stone-0f9793c00.yml)

[![build and deploy](https://github.com/nathanpee2006/basketball-progress-tracker/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/nathanpee2006/basketball-progress-tracker/actions/workflows/build-and-deploy.yml)



## Deployment

- **Live link:** https://basketball-progress-tracker.me/
- **Scalar API Documentation UI**: https://basketball-progress-tracker-fmbvd6dqeuhzeubj.newzealandnorth-01.azurewebsites.net/scalar/#description/introduction

## Introduction

As someone who finds it hard to be consistent and frustating to know whether I am improving in my basketball skills or not, I built this app for myself.

A web application that tracks individual progress in basketball in a gamified way. You can log sessions which allows you to track how many shots you made and missed. As long as you remain consistent you can measure your progress (i.e. shooting percentages in specific zones such as free throw, paint, midrange, and three pointers) over time against other players. This is for people wanting to know if they are improving over time through data.

## Theme Connection

The Basketball Progress Tracker app relates to the Gamification theme as it contains:
- Streaks: empowers the player to maintain it by being consistent 
- Weekly Leaderboards: makes it competitive and motivates others to do better and outwork them
- Achievement Badges: provides clear markers of progress and rewards the player for achieving them

## What Makes This Unique

Other basketball progress tracker apps feature personal progress tracking and streaks, but not a lot have a leaderboard. The leaderboard system within my app is a weekly leaderboard rather than all-time. I chose this because rather than a new user feeling overwhelmed of a massive gap in an all-time leaderboard the weekly leaderboard allows them to stand on the same starting point as everyone, which makes it more competitive.

## Advanced Features

- [✅] Feature 1: Dockerizing project with Docker
- [✅] Feature 2: Support for theme switching (light/dark mode)
- [✅] Feature 3: API optimizations (call data access APIs asynchronously, no-tracking queries when accessing data for read-only purposes, SQL joins instead of N+1 queries, CancellationToken to stop long-running/resource-heavy/asynchronous operations when their results are no longer needed)   

## Self-Reflection

### What I would have done differently

**Data fetching**: I hand-rolled data fetching with fetch(), which meant writing the same isLoading/isError/data boilerplate in every hook, and every page navigation triggered a fresh network request even when the data hadn't changed (i.e. going back to a session list I'd already loaded). Next time I'd use React Query specifically for its caching: it would let me reuse previously fetched data instead of making another request the backend every time I navigated to a page, and it would remove the repeated state-management boilerplate across hooks.

**Hook duplication**: useSessions, useSession, useShootingAnalytics, and other GET-related hooks all contain near-identical logic for making a GET request and tracking loading/error state. I should have factored this into a generic useGet<T>() hook. (and probably used React Query with it)

**Feature I wish I added**: I wanted to add an NBA 2K-style player rating feature but didn't have time to implement it as I still needed to implement other features such as the streaks and leaderboard.
