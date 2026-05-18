import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { AccountForm } from '@/components/account/AccountForm';

describe('AccountForm validation UX', () => {
  it('marks the email input as aria-invalid when it has an error', async () => {
    const user = userEvent.setup();
    render(
      <AccountForm
        title="Create Account"
        submitLabel="Create Account"
        onSubmit={vi.fn()}
      />
    );

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(screen.getByLabelText(/email/i)).toHaveAttribute(
      'aria-invalid',
      'true'
    );
  }, 10000);

  it('links the email input to its error via aria-describedby', async () => {
    const user = userEvent.setup();
    render(
      <AccountForm
        title="Create Account"
        submitLabel="Create Account"
        onSubmit={vi.fn()}
      />
    );

    await user.click(screen.getByRole('button', { name: /create account/i }));

    const input = screen.getByLabelText(/email/i);
    const errorId = input.getAttribute('aria-describedby');

    expect(errorId).toBeTruthy();
    expect(document.getElementById(errorId!)).toHaveTextContent(
      /email is required/i
    );
  }, 10000);

  it('uses invitational tone for validation messages', async () => {
    const user = userEvent.setup();
    render(
      <AccountForm
        title="Create Account"
        submitLabel="Create Account"
        onSubmit={vi.fn()}
      />
    );

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(screen.getByText('Email is required')).toBeInTheDocument();
    expect(screen.getByText('Password is required')).toBeInTheDocument();
    expect(
      screen.getByText('Confirm password is required')
    ).toBeInTheDocument();
  }, 10000);

  it('renders field errors with role="alert" for screen readers', async () => {
    const user = userEvent.setup();
    render(
      <AccountForm
        title="Create Account"
        submitLabel="Create Account"
        onSubmit={vi.fn()}
      />
    );

    await user.click(screen.getByRole('button', { name: /create account/i }));

    const alerts = screen.getAllByRole('alert');
    expect(alerts.length).toBeGreaterThanOrEqual(3);
  }, 10000);

  it('clears aria-invalid when fields are corrected and resubmitted', async () => {
    const user = userEvent.setup();
    render(
      <AccountForm
        title="Create Account"
        submitLabel="Create Account"
        onSubmit={vi.fn()}
      />
    );

    // First submit → errors appear
    await user.click(screen.getByRole('button', { name: /create account/i }));
    expect(screen.getByLabelText(/email/i)).toHaveAttribute(
      'aria-invalid',
      'true'
    );

    // Fix fields
    await user.type(screen.getByLabelText(/email/i), 'frank@example.com');
    await user.type(screen.getByLabelText(/^password$/i), 'Password123!');
    await user.type(
      screen.getByLabelText(/confirm password/i),
      'Password123!'
    );

    // Submit again → errors cleared
    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(screen.getByLabelText(/email/i)).not.toHaveAttribute(
      'aria-invalid'
    );
  }, 10000);

  it('renders form-level errors with role="alert"', () => {
    render(
      <AccountForm
        title="Create Account"
        submitLabel="Create Account"
        onSubmit={vi.fn()}
        errors={{ form: 'Something went wrong. Please try again.' }}
      />
    );

    expect(screen.getByRole('alert')).toHaveTextContent(
      'Something went wrong. Please try again.'
    );
  });
});
