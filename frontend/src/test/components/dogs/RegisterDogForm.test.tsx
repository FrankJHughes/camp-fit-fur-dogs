import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import RegisterDogForm from '@/components/dogs/RegisterDogForm';

function setup(overrides: Partial<{
  run: (data: any) => void;
  errors: Record<string, string>;
  isSubmitting: boolean;
}> = {}) {
  const run = vi.fn();
  const command = {
    run,
    errors: {},
    isSubmitting: false,
    ...overrides,
  };

  render(<RegisterDogForm command={command} />);

  return { user: userEvent.setup(), run, command };
}

describe('RegisterDogForm', () => {
  it('renders the form with all fields and a submit button', () => {
    setup();

    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/breed/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/date of birth/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/sex/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /register/i })).toBeInTheDocument();
  });

  it('calls run with form data when all fields are filled', async () => {
    const { user, run } = setup();

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(run).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  });

  it('renders validation errors passed via command.errors', () => {
    setup({
      errors: { name: 'Name is already taken' },
    });

    expect(screen.getByText('Name is already taken')).toBeInTheDocument();
  });

  it('does not call run when validation fails', async () => {
    const { user, run } = setup();

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(run).not.toHaveBeenCalled();
  });

  it('calls run after correcting validation errors', async () => {
    const { user, run } = setup();

    // First submit → validation fails
    await user.click(screen.getByRole('button', { name: /register/i }));
    expect(run).not.toHaveBeenCalled();

    // Fix fields
    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(run).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  });
});
