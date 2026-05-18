// src/lib/account/validateAccountForm.ts

export interface AccountFormValues {
  email: string;
  password: string;
  confirmPassword: string;
}

export function validateAccountForm(
  values: AccountFormValues
): Record<string, string> {
  const errors: Record<string, string> = {};

  const email = values.email.trim();
  const password = values.password;
  const confirmPassword = values.confirmPassword;

  if (!email) {
    errors.email = 'Email is required';
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    errors.email = 'Invalid email';
  }

  if (!password) {
    errors.password = 'Password is required';
  } else if (password.length < 8) {
    errors.password = 'Password must be at least 8 characters';
  }

  if (!confirmPassword) {
    errors.confirmPassword = 'Confirm password is required';
  } else if (password !== confirmPassword) {
    errors.confirmPassword = 'Passwords do not match';
  }

  return errors;
}
