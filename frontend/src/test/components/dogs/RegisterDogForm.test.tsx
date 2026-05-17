import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RegisterDogForm } from '@/components/dogs/RegisterDogForm';
import type { RegisterDogCommand } from '@/api/dogs/registerDog';

function setup(overrides: Partial<{
  submit: (data: RegisterDogCommand) => void;
  errors: Record<string, string>;
  isSubmitting: boolean;
}> = {}) {
  const submit = vi.fn();
  const command = {
    submit,
    errors: {},
    isSubmitting: false,
    ...overrides,
  };

  render(<RegisterDogForm command={command} />);

  return { user: userEvent.setup(), submit, command };
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

  it('calls submit with form data when all fields are filled', async () => {
    const { user, submit } = setup();

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(submit).toHaveBeenCalledWith({
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

  it('does not call submit when validation fails', async () => {
    const { user, submit } = setup();

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(submit).not.toHaveBeenCalled();
  });

  it('calls submit after correcting validation errors', async () => {
    const { user, submit } = setup();

    // First submit → validation fails
    await user.click(screen.getByRole('button', { name: /register/i }));
    expect(submit).not.toHaveBeenCalled();

    // Fix fields
    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(submit).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  });
});
