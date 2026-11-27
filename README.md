<img src="https://raw.githubusercontent.com/axuno/ClubSite/main/club-logo.png" width=100 height=100 alt="" />

# ClubSite

[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/axuno/MailMergeLib/blob/master/License.txt)
[![build + test](https://github.com/axuno/ClubSite/workflows/build%20+%20test/badge.svg)](https://github.com/axuno/ClubSite/actions/workflows/build%20+%20test.yml)

## About this project
This is the source code for a sports club website. The C# AspNetCore project is using [PiranhaCMS](https://piranhacms.org/), 
which is a lightweight, unobtrusive, cross-platform and open source framework. The framework brings a simple, intuitive and powerful user interface, 
and makes creating content enjoyable and fun, even for non technicians.

## Live website
Have a look at the website: [https://www.volleyballclub.de/](https://www.volleyballclub.de/)

## Calendar page
The calendar page displays events from Piranha-managed `.ics` documents in Europe/Berlin time.
“Vergangene Termine” covers the last six months; “Alle Termine” covers two months before
to ten months after the current time. Upcoming events are searched up to ten months ahead.

For performance, the service expands at most 2,000 occurrences per calendar and returns
at most 300 events across calendars, sorted by start time. Both limits apply before team,
category and display-mode filters, so filtered results may also be incomplete. The page
shows a warning when either limit truncates the requested range.
