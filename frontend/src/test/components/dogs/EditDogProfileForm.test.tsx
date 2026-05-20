import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { EditDogProfileForm } from '@/components/dogs/EditDogProfileForm';

const initialData = {
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('EditDogProfileForm', () => {
  it('renders the form with all fields and a save button', () => {
    const command = {
      run: vi.fn(),
      errors: {},
      isSubmitting: false,
    };

    render(
      <EditDogProfileForm initialValues={initialData} command={command} />,
    );
    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/breed/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/date of birth/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/sex/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /save/i })).toBeInTheDocument();
  });

  it('pre-populates fields with initial data', () => {
    const command = {
      run: vi.fn(),
      errors: {},
      isSubmitting: false,
    };

    render(
      <EditDogProfileForm initialValues={initialData} command={command} />,
    );
    expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
    expect(screen.getByLabelText(/breed/i)).toHaveValue('Golden Retriever');
    expect(screen.getByLabelText(/date of birth/i)).toHaveValue('2023-06-15');
    expect(screen.getByLabelText(/sex/i)).toHaveValue('Male');
  });

  it('calls onSubmit with updated form data', async () => {
    const run = vi.fn();
    const command = {
      run,
      errors: {},
      isSubmitting: false,
    };
    const user = userEvent.setup();
    render(
      <EditDogProfileForm initialValues={initialData} command={command} />,
    );

    await user.clear(screen.getByLabelText(/name/i));
    await user.type(screen.getByLabelText(/name/i), 'Max');
    await user.click(screen.getByRole('button', { name: /save/i }));

    expect(run).toHaveBeenCalledWith({
      name: 'Max',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  }, 10000);

  it('renders validation errors passed via errors prop', () => {
    const command = {
      run: vi.fn(),
      errors: { name: 'Name is already taken' },
      isSubmitting: false,
    };

    render(
      <EditDogProfileForm initialValues={initialData} command={command} />,
    );
    expect(screen.getByText('Name is already taken')).toBeInTheDocument();
  });

  it('disables the save button when isSubmitting is true', () => {
    const command = {
      run: vi.fn(),
      errors: {},
      isSubmitting: true,
    };

    render(
      <EditDogProfileForm initialValues={initialData} command={command} />,
    );
    expect(screen.getByRole('button', { name: /save/i })).toBeDisabled();
  });
});
