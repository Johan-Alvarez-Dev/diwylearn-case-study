# Technical Decisions

## One account, multiple roles

A user may learn and create. This avoids duplicate profiles but requires combined role/ownership policies.

## Vertical slices without premature ceremony

Contracts and services live with their feature. This keeps ownership clear without introducing a mediator merely for structure.

## Server-owned progress

Progress and counts come from persisted facts and SQL projections, not hard-coded UI values.

## AI proposes, creators approve

Generated course structure is reviewed before persistence, preserving editorial control.

## Security before feature expansion

Sanitization, rate limiting, session behavior, and efficient reads are prioritized before additional commercial modules.
