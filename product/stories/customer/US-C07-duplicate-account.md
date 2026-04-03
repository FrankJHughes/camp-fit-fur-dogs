# US-C07 — Duplicate Account Attempt

## Intent

As a customer, I want to be guided clearly if I try to create an account that already exists, so that I understand what happened and what to do next.

## Value

Protects the customer from confusion and self-blame when the system detects a duplicate. Reinforces trust by treating the attempt as a natural occurrence, not an error on the customer's part.

## Acceptance Criteria

- Customer is informed that an account with their information already exists
- The message does not blame or shame the customer for the attempt
- Customer is offered a clear path forward (e.g., sign in, recover access)
- No internal identifiers, error codes, or system concepts are exposed
- The experience feels like guidance, not rejection

## Emotional Guarantees

- EG-02 No blame
