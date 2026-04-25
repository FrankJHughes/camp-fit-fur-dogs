import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RegisterDogForm } from '@/components/dogs/RegisterDogForm';

describe('RegisterDogForm', () => {
  it('renders the form with all fields and a submit button', () => {
    render(<RegisterDogForm onSubmit={vi.fn()} />);

    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/breed/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/date of birth/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/sex/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /register/i })).toBeInTheDocument();
  });

  it('calls onSubmit with form data when all fields are filled', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(onSubmit).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  }, 10000);

  it('renders validation errors passed via errors prop', () => {
    render(
      <RegisterDogForm
        onSubmit={vi.fn()}
        errors={{ name: 'Name is already taken' }}
      />
    );

    expect(screen.getByText('Name is already taken')).toBeInTheDocument();
  });

  it('shows error when name is empty', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByText('Name is required')).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  }, 10000);

  it('shows error when breed is empty', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByText('Breed is required')).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  }, 10000);

  it('shows error when date of birth is empty', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByText('Date of birth is required')).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  }, 10000);

  it('shows error when sex is not selected', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByText('Sex is required')).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  }, 10000);

  it('does not call onSubmit when validation fails', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(onSubmit).not.toHaveBeenCalled();
  }, 10000);

  it('calls onSubmit after correcting validation errors', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<RegisterDogForm onSubmit={onSubmit} />);

    await user.click(screen.getByRole('button', { name: /register/i }));
    expect(screen.getByText('Name is required')).toBeInTheDocument();

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(onSubmit).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  }, 10000);
});
