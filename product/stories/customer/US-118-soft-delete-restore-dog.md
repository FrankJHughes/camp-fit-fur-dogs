# US-118 — Soft Delete & Restore Dog

## Intent

As an owner, I should be able to restore a dog I previously removed so that accidental removals don't cause permanent data loss.

## Value

Hard delete (US-032) is the simplest correct implementation today, but real users make mistakes. Soft delete adds a safety net — removed dogs are hidden but recoverable within a grace period, reducing support burden and owner anxiety.

## Acceptance Criteria

- Removing a dog marks it as soft-deleted rather than physically deleting the row
- Soft-deleted dogs no longer appear in any owner-facing views (dog list, profiles)
- Owner can view a list of recently removed dogs within a configurable grace period
- Owner can restore a soft-deleted dog, returning it to full visibility
- After the grace period expires, soft-deleted dogs are permanently purged (or remain archived per policy decision)
- Existing US-032 Remove Dog flow is unchanged from the owner's perspective — only the persistence mechanism changes

## Emotional Guarantees

- EG-01 No Surprises — restoration is discoverable, not hidden
- EG-03 Calm Protection — mistakes are recoverable, anxiety is reduced
- EG-06 Explicit Risk — grace period and permanence are stated clearly

## Notes

- Prerequisite: US-032 (Remove Dog) must ship first with hard delete
- This story replaces the hard delete with a soft-delete mechanism behind the scenes
- Domain change: `Dog` aggregate gains an `IsRemoved` flag and `RemovedAt` timestamp
- All queries must filter out `IsRemoved == true` dogs
- Consider: should the grace period be configurable per-owner or system-wide?
- Consider: purge strategy — background job vs. on-read cleanup
