# US-040 — Reassurance on Failure

## Intent

As a customer, I want the system to reassure me when something goes wrong, so that I don't feel like I caused the problem or that my data is at risk.

## Value

Failures happen. The emotional difference between a system that says "something went wrong, but your data is safe and you can try again" and one that shows a raw error is the difference between trust and abandonment.

## Acceptance Criteria

- When the system encounters an error, the customer receives a reassuring, human-readable message
- The message makes clear the customer did nothing wrong
- The message confirms that existing data has not been affected
- A recovery path is offered (retry, return, contact support)
- No technical jargon, stack traces, or error codes are shown

## Emotional Guarantees

- EG-02 No blame
