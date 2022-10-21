---
name: Deliverable Template
about: This is for a project deliverable.
title: Add a deliverable title.
labels: help wanted, question, WIP
assignees: ''

---

# Deliverable (TODO: Add Name)

## Status
|                |                                         |
|----------------|-----------------------------------------|
| Asignee        | TODO: Enter you name                    |
| Reviewer       | TODO: add reviwer name (when reviewed)  |
| Estimate       | TODO: enter the estimated business days |
| Working Branch | TODO: enter the working branch name     |

## Purpose

We understand the ***primary*** factor for successful development is clear communication. The purpose of this pre-construction checklist is to ensure.
1. We all agree on the requirements and deliverables before we write code.
2. The work meets the underlying business goals driving the work. 
3. We scope, estimate, and plan with accuracy.

## Tasks

1. [ ] Write Overview
2. [ ] Write Acceptance Criteria Decision Tables
3. [ ] Write Acceptance Criteria Natural Language requirements (EARS syntax)
4. [ ] List Risks (including rankings of frequency and occurrence, and overall score)
5. [ ] List Risk Mitigations for Every Appropriate Risk (Ears syntax) and Update risk rankings 
6. [ ] Request Review on Risks & Risk Mitigations
7. [ ] Update Risks & Risk Mitigations from Feedback
8. [ ] Write Testing Strategy Tasks
9. [ ] Request Review on Test Strategy
10. [ ] Update Test Strategy from Feedback
11. [ ] Add any remaining tasks required to deliver to this list
12. [ ] Provide an estimate on work (granularity work days or weeks, depending on the project)
12. [ ] As a feature is completed, port the decision table and EARS statement to the appropriate application documentation file.

## Deliverable Overview

> Instructions: 
> * Write a brief description of the deliverable goes here (generally 1-2 paragraphs or 4-8 sentences)
> * Start working on the task list (below)## Acceptance Criteria

This section identifies the acceptance criteria for the deliverable.

> ### Note
> First, we use decision table notation to outline logic and information.
> We compliment the decision tables with additional natural language requirements using the [EARS](https://github.com/Vetrify/lexala/blob/main/docs/ears.md) language conventions to ensure:
> * Acceptance criteria is testable/measurable. They should be easy to prove. And the results of these tests/measurements should leave no room for interpretation. The acceptance test must provide a clear yes/no or pass/non-results.
 > * Criteria is clear and concise. We don't need extensive documentation and keep our acceptance criteria as simple and clear as possible.
> * Everyone understands the acceptance criteria. Our criteria are useless if we all cannot understand them.
> * We use formal analysis to calculate the steps needed to complete this document using [our process](https://github.com/Vetrify/truthshield-docs/blob/main/engineering-process.md) and metrics of [severity](https://github.com/Vetrify/truthshield-docs/blob/main/engineering-process.md#severity-scale) and [frequency](https://github.com/Vetrify/truthshield-docs/blob/main/engineering-process.md#frequency-scale) to [determine the appropriate action](https://github.com/Vetrify/truthshield-docs/blob/main/engineering-process.md#action-grid) to take.
> ### Instructions
> * use the template below to document the measurable acceptance criteria for completion of this deliverable.

[-](*{table id}*)
|             |             |             |
|-------------|-------------|-------------|
| Condition 1 | {statement} | {statement} |
| Condition 2 | {statement} | {statement} |
| > Action 1  | {statement} | {statement} |
| > Action 2  | {statement} | {statement} |


- Risk: {description} [severity: {severity}, frequency: {frequency}] 
  - [{EARS COMPENSATION Statement}](#id#)
  - [{EARS COMPENSATION Statement}](#id#)
- Risk: {description} [severity: {severity}, frequency: {frequency}] 
  - [{EARS COMPENSATION Statement}](#id#)
  - [{EARS COMPENSATION Statement}](#id#)

- [{EARS Statement}](#id#)
> - Risk: {description} [severity: {severity}, frequency: {frequency}] 
>   - [{EARS COMPENSATION Statement}](#id#)
>   - [{EARS COMPENSATION Statement}](#id#)
> - Risk: {description} [severity: {severity}, frequency: {frequency}] 
>   - [{EARS COMPENSATION Statement}](#id#)
>   - [{EARS COMPENSATION Statement}](#id#)

- [{EARS Statement}](#id#)
> - Risk: {description} [criticality frequency score] 
>   - [{EARS COMPENSATION Statement}](#id#)
>   - [{EARS COMPENSATION Statement}](#id#)


> ### Example (pass 1)
> - [The build system will produce linting report for all typescript code.](#745c5cc2-c922-4880-bd75-1a6fe59e9f6c#)
> - [The user can see their group membership.](#c0226ff8-2919-4947-96f4-da79a20a4796#)

> ### Example (pass 2)
> - [The build system will produce linting report for all typescript code.](#745c5cc2-c922-4880-bd75-1a6fe59e9f6c#)
>   - Risk: the user is not authenticated [frequency: likely, severity: moderate]
> - [The user can see their group membership.](#c0226ff8-2919-4947-96f4-da79a20a4796#)

> ### Example (pass 3)
> - [The build system will produce linting report for all typescript code.](#745c5cc2-c922-4880-bd75-1a6fe59e9f6c#)
>   - Risk: the user is not authenticated [frequency: remote, severity: moderate]
>     - [When a user requests linting, if the user is not authenticated, the build system will redirect the user to first be authenticated.](#343a8ecd-5acf-4702-a0bc-853ee81eb03f#)
> - [The user can see their group membership.](#c0226ff8-2919-4947-96f4-da79a20a4796#)


> Note: for system-level NFR(s) - a system-level deliverable template can be used to cover many cross-cutting cases such as reliability, redundancy, scalability, etc.  If the deliverable is a feature, then the risks should be in the context of the deliverable, not necessarily higher-order system-level concerns.

### Additional Testing Tasks
> ### Instructions
>  Check off the tasks (below) and risks (above) once they have been addressed.

- [ ] **Test 01** 
  > *Brief* (required)
  > * Detail (optional)
  > * Description (optional)
  > - [ ] Sub-Task 1 (optional)
  >   * Sub-Task Description 1 (optional)
  >   * Sub-Task Description 2 (optional)
  > - [ ] Sub-Task 2 (optional)
  >   * Sub-Task Description 1 (optional)
  >   * Sub-Task Description 2 (optional)
- [ ] **Test 02**
  > *Brief* (required)
  > * Detail (optional)
  > * Description (optional)
  > - [ ] Sub-Task 1 (optional)
  >   * Sub-Task Description 1 (optional)
  >   * Sub-Task Description 2 (optional)
  > - [ ] Sub-Task 2 (optional)
  >   * Sub-Task Description 1 (optional)
  >   * Sub-Task Description 2 (optional)
- [ ] **Test 03**
  > *Brief* (required)
  > * Detail (optional)
  > * Description (optional)
  > - [ ] Sub-Task 1 (optional)
  >   * Sub-Task Description 1 (optional)
  >   * Sub-Task Description 2 (optional)
  > - [ ] Sub-Task 2 (optional)
  >   * Sub-Task Description 1 (optional)
  >   * Sub-Task Description 2 (optional)
