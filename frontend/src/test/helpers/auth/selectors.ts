import { screen } from "@testing-library/react";

// Inputs
export const selectors = {
  email: () => screen.getByLabelText(/email/i),
  password: () => screen.getByLabelText(/^password$/i),
  confirmPassword: () => screen.getByLabelText(/confirm password/i),
  button: () => screen.getByRole("button", { name: /create account/i }),

  // Validation errors
  emailError: () => screen.queryByText(/^email is required$/i),
  passwordError: () => screen.queryByText(/^password is required$/i),
  confirmPasswordError: () => screen.queryByText(/^confirm password is required$/i),

  invalidEmailError: () => screen.queryByText(/^invalid email$/i),
  shortPasswordError: () => screen.queryByText(/^password must be at least 8 characters$/i),
  passwordMismatchError: () => screen.queryByText(/^passwords do not match$/i),

  // Success message
  successMessage: () => screen.queryByText(/account created successfully/i),
};
