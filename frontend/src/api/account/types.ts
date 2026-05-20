// src/api/account/types.ts
/**
 * API DTO types for the account endpoints.
 *
 * Keep DTOs here to avoid circular imports between API wrappers and
 * other modules that also export functions.
 */

export type CreateAccountCommand = {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  password: string;
};
