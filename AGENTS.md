# BMS Agent Instructions

## Before working

- Check the current Git branch and Git status.
- Do not overwrite unrelated local changes.
- Understand the affected code flow before making changes.

## Documentation

For overall project context read:

- BMS_PROJECT_KNOWLEDGE.md
- BMS_ARCHITECTURE_DEVELOPER_GUIDE.md

For Preventive Maintenance work read:

- BMS_PREVENTIVE_MAINTENANCE_BACKEND.md

For PM API testing read:

- BMS_PM_USER_GUIDE_AND_API_TESTS.md

## Source of truth

Current source code is the primary source of truth.
Documentation may become outdated.
If code and documentation disagree, report the inconsistency.

## Development rules

- Follow the existing CQRS + MediatR architecture.
- Keep controllers thin.
- Follow existing Domain/Application/Infrastructure boundaries.
- Preserve authorization and permission checks.
- Do not expose or commit secrets.
- Do not introduce new dependencies without justification.
- Keep changes focused on the requested task.
- Do not modify Worker, Modbus, Telemetry or Historian unless required by the task.
- Preserve existing concurrency and transaction protections.

## Verification

Before finishing:

- Review the final diff.
- Run the relevant build and tests.
- Report warnings, assumptions and unresolved issues.
- Update relevant documentation when behavior or architecture changes.
