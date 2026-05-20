import { CreateAccountSchema, type CreateAccountValues } from './createAccountSchema';

/**
 * Validates Create Account form values using the Zod schema.
 *
 * Returns a flat error map:
 *   { firstName: "message", lastName: "message", email: "message", ... }
 *
 * If validation passes, returns an empty object.
 */
export function validateAccountForm(
  values: CreateAccountValues
): Record<string, string> {
  const result = CreateAccountSchema.safeParse(values);

  if (result.success) {
    return {};
  }

  const errors: Record<string, string> = {};

  // Flatten Zod errors into a simple { field: message } map
  for (const issue of result.error.issues) {
    const field = issue.path[0];
    if (typeof field === 'string' && !errors[field]) {
      errors[field] = issue.message;
    }
  }

  return errors;
}
