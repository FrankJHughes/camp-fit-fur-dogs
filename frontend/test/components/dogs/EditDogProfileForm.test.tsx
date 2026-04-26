import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { EditDogProfileForm } from '@/components/dogs/EditDogProfileForm';

const initialData = {
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('EditDogProfileForm', () => {
  it('renders the form with all fields and a save button', () => {
    render(
      <EditDogProfileForm initialData={initialData} onSubmit={vi.fn()} />,
    );
    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/breed/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/date of birth/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/sex/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /save/i })).toBeInTheDocument();
  });

  it('pre-populates fields with initial data', () => {
    render(
      <EditDogProfileForm initialData={initialData} onSubmit={vi.fn()} />,
    );
    expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
    expect(screen.getByLabelText(/breed/i)).toHaveValue('Golden Retriever');
    expect(screen.getByLabelText(/date of birth/i)).toHaveValue('2023-06-15');
    expect(screen.getByLabelText(/sex/i)).toHaveValue('Male');
  });

  it('calls onSubmit with updated form data', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(
      <EditDogProfileForm initialData={initialData} onSubmit={onSubmit} />,
    );

    await user.clear(screen.getByLabelText(/name/i));
    await user.type(screen.getByLabelText(/name/i), 'Max');
    await user.click(screen.getByRole('button', { name: /save/i }));

    expect(onSubmit).toHaveBeenCalledWith({
      name: 'Max',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  }, 10000);

  it('renders validation errors passed via errors prop', () => {
    render(
      <EditDogProfileForm
        initialData={initialData}
        onSubmit={vi.fn()}
        errors={{ name: 'Name is already taken' }}
      />,
    );
    expect(screen.getByText('Name is already taken')).toBeInTheDocument();
  });

  it('disables the save button when isSubmitting is true', () => {
    render(
      <EditDogProfileForm
        initialData={initialData}
        onSubmit={vi.fn()}
        isSubmitting
      />,
    );
    expect(screen.getByRole('button', { name: /save/i })).toBeDisabled();
  });
});