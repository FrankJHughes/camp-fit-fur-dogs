import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { CreateAccountForm } from '@/components/account/CreateAccountForm';

describe('CreateAccountForm wrapper', () => {
  it('adapts CreateAccountValues into CreateAccountCommand and calls command.run', async () => {
    const user = userEvent.setup();

    const run = vi.fn();
    render(
      <CreateAccountForm
        command={{
          run,
          errors: {},
          isSubmitting: false,
        }}
      />
    );

    // Fill out the inner AccountForm
    await user.type(screen.getByLabelText(/first name/i), 'Frank');
    await user.type(screen.getByLabelText(/last name/i), 'Hughes');
    await user.type(screen.getByLabelText(/email/i), 'frank@example.com');

    // Updated: must be 10 digits
    await user.type(screen.getByLabelText(/phone/i), '9165551234');

    await user.type(screen.getByLabelText(/^password$/i), 'Password123!');
    await user.type(
      screen.getByLabelText(/confirm password/i),
      'Password123!'
    );

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(run).toHaveBeenCalledWith({
      firstName: 'Frank',
      lastName: 'Hughes',
      email: 'frank@example.com',
      phone: '9165551234', // Updated expected value
      password: 'Password123!',
    });
  });

  it('passes errors through to AccountForm', () => {
    render(
      <CreateAccountForm
        command={{
          run: vi.fn(),
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
          run: vi.fn(),
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
