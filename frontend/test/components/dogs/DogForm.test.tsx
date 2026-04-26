import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { DogForm } from '../../../src/components/dogs/DogForm';

describe('DogForm validation UX (US-035)', () => {
  it('marks the name input as aria-invalid when it has an error', async () => {
    const user = userEvent.setup();
    render(
      <DogForm title="Register Dog" submitLabel="Register" onSubmit={vi.fn()} />,
    );
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByLabelText(/name/i)).toHaveAttribute(
      'aria-invalid',
      'true',
    );
  }, 10000);

  it('links the name input to its error via aria-describedby', async () => {
    const user = userEvent.setup();
    render(
      <DogForm title="Register Dog" submitLabel="Register" onSubmit={vi.fn()} />,
    );
    await user.click(screen.getByRole('button', { name: /register/i }));

    const input = screen.getByLabelText(/name/i);
    const errorId = input.getAttribute('aria-describedby');
    expect(errorId).toBeTruthy();
    expect(document.getElementById(errorId!)).toHaveTextContent(/please enter/i);
  }, 10000);

  it('uses invitational tone for validation messages', async () => {
    const user = userEvent.setup();
    render(
      <DogForm title="Register Dog" submitLabel="Register" onSubmit={vi.fn()} />,
    );
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByText("Please enter your dog's name")).toBeInTheDocument();
    expect(screen.getByText('Please enter a breed')).toBeInTheDocument();
    expect(screen.getByText('Please enter a date of birth')).toBeInTheDocument();
    expect(screen.getByText('Please select a sex')).toBeInTheDocument();
  }, 10000);

  it('renders field errors with role="alert" for screen readers', async () => {
    const user = userEvent.setup();
    render(
      <DogForm title="Register Dog" submitLabel="Register" onSubmit={vi.fn()} />,
    );
    await user.click(screen.getByRole('button', { name: /register/i }));

    const alerts = screen.getAllByRole('alert');
    expect(alerts.length).toBeGreaterThanOrEqual(4);
  }, 10000);

  it('clears aria-invalid when field is corrected and resubmitted', async () => {
    const user = userEvent.setup();
    render(
      <DogForm title="Register Dog" submitLabel="Register" onSubmit={vi.fn()} />,
    );
    await user.click(screen.getByRole('button', { name: /register/i }));
    expect(screen.getByLabelText(/name/i)).toHaveAttribute('aria-invalid', 'true');

    await user.type(screen.getByLabelText(/name/i), 'Buddy');
    await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
    await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByLabelText(/name/i)).not.toHaveAttribute('aria-invalid');
  }, 10000);

  it('renders form-level errors with role="alert"', () => {
    render(
      <DogForm
        title="Register Dog"
        submitLabel="Register"
        onSubmit={vi.fn()}
        errors={{ form: 'Something went wrong. Please try again.' }}
      />,
    );
    expect(screen.getByRole('alert')).toHaveTextContent(
      'Something went wrong. Please try again.',
    );
  });
});