import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { CreateAccountForm } from '@/components/account/CreateAccountForm';

describe('CreateAccountForm wrapper', () => {
  it('adapts CreateAccountValues into CreateAccountCommand and calls command.submit', async () => {
    const user = userEvent.setup();

    const submit = vi.fn();
    render(
      <CreateAccountForm
        command={{
          submit,
          errors: {},
          isSubmitting: false,
        }}
      />
    );

    // Fill out the inner AccountForm
    await user.type(screen.getByLabelText(/email/i), 'frank@example.com');
    await user.type(screen.getByLabelText(/^password$/i), 'Password123!');
    await user.type(
      screen.getByLabelText(/confirm password/i),
      'Password123!'
    );

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(submit).toHaveBeenCalledWith({
      email: 'frank@example.com',
      password: 'Password123!',
      confirmPassword: 'Password123!',
    });
  });

  it('passes errors through to AccountForm', () => {
    render(
      <CreateAccountForm
        command={{
          submit: vi.fn(),
          errors: { form: 'Server exploded' },
          isSubmitting: false,
        }}
      />
    );

    expect(screen.getByRole('alert')).toHaveTextContent('Server exploded');
  });

  it('passes isSubmitting through to AccountForm', () => {
    render(
      <CreateAccountForm
        command={{
          submit: vi.fn(),
          errors: {},
          isSubmitting: true,
        }}
      />
    );

    expect(
      screen.getByRole('button', { name: /create account/i })
    ).toBeDisabled();
  });
});
