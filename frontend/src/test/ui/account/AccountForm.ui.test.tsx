import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { apiClientMock } from '@/test/setup';
import { fillCreateAccountForm } from '@/test/helpers/account/fillCreateAccountForm';

describe('AccountForm (UI)', () => {
  beforeEach(() => {
    apiClientMock.post.mockClear();
  });

  async function loadForm() {
    const mod = await import('@/components/account/AccountForm');
    return mod.AccountForm || mod.default;
  }

  const defaultProps = {
    title: "Create Account",
    submitLabel: "Create Account",
    onSubmit: vi.fn(),
  };

  it('renders all fields', async () => {
    const AccountForm = await loadForm();
    render(<AccountForm {...defaultProps} />);

    expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/last name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/phone/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/^password$/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/confirm password/i)).toBeInTheDocument();
  });

  it('submits valid data', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: true,
      data: { success: true },
    });

    const AccountForm = await loadForm();
    const user = userEvent.setup();

    render(<AccountForm {...defaultProps} />);

    await fillCreateAccountForm(user);

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(defaultProps.onSubmit).toHaveBeenCalledWith({
      firstName: 'Frank',
      lastName: 'Hughes',
      email: 'frank@example.com',
      phone: '916-555-1234', // updated to match helper + schema
      password: 'Password123!',
      confirmPassword: 'Password123!',
    });
  });

  it('shows client-side validation errors', async () => {
    const AccountForm = await loadForm();
    const user = userEvent.setup();

    render(<AccountForm {...defaultProps} />);

    // Trigger validation with bad email
    await user.type(screen.getByLabelText(/email/i), 'not-an-email');
    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(await screen.findByText(/invalid email address/i)).toBeInTheDocument();
  });
});
