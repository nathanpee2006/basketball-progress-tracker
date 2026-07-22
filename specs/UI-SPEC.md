# UI Summary — Basketball Progress Tracker (v1)

This document summarizes the UI decisions agreed today. It's mobile-first and aimed at fast session logging, clear streak visibility, and lightweight analytics.

## Top-level
- Navigation: bottom tabs (mobile): Dashboard | Sessions | Analytics
- Color: neutral black/white surfaces with orange as the accent/action color (primary CTA, highlights).

## Dashboard (default home)
- Shows current weekly Streak (ISO week, player timezone) and this-week status.
- Primary "Log Session" CTA (prominent orange button). Quick access (one tap) opens the Session form with sensible defaults (today's date, player's timezone).
- Recent Sessions list (most recent first) with compact cards.
- Quick analytics snapshot: weakest zone, FT% trend, latest drill delta.

## Session entry
- Default: Full Session form with optional collapsible sections for:
  - Zone shooting (paint, midrange, three-point) — make/attempt numeric fields
  - Free throws (makes/attempts)
  - Drills (list of named timed drills; Add Drill button)
- Quick log = fast access to full form (not a reduced schema).
- Date picker: if chosen date is outside current ISO week, show inline backfill warning: "Backfill Session: saved to history, won’t extend Streak".
- Editing: numeric inputs + interactive basketball court are fully synced (editing either updates the other).
- Court visualization: interactive, shows makes/attempts per zone and percentage; appears beside inputs and in Session detail.

## Court visualization behavior
- Dual-edit sync: changes on the court or in numeric fields stay synced in real time.
- Each zone displays makes, attempts, and the derived percentage.
- Tapping a zone opens a small action sheet with buttons: Make, Miss, Undo, and an Edit option to open the numeric editor.
- Make/Miss buttons increment counters immediately (optimistic update); Undo reverts the last action. The numeric editor allows precise entry and keeps the court visualization synced.
- Court appears beside inputs in the form and in Session details; both support interactive input and viewing.

## Sessions list & details
- Session card actions: 3-dot menu (View | Edit | Delete). Delete requires confirmation modal.
- Tapping a session opens details with court visualization, derived percentages, drill list, and edit button.

## Analytics (v1)
- Metric cards + small trend lines (mobile-optimized): weakest zone, career zone %, FT trend, drill trends, shot volume by zone.
- Avoid heavy interactive charts in v1 to keep performance and clarity on phones.

## Accessibility & Data
- All percentages are derived on read; UI displays derived percentages but they are not stored.
- Ensure color contrast for orange on white/black; provide accessible labels for court zones and numeric fields.

## Developer notes
- Mobile-first design; bottom nav should be persistent.
- Session form must validate numeric fields (non-negative integers, attempts >= makes).
- Streak rule: only sessions dated in the current ISO week extend streaks; show warning for backfills.

---

If anything here should be changed before tomorrow, note it in the PRD or reply in the discussion thread.
