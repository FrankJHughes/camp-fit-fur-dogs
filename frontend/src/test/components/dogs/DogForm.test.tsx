import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { DogForm } from '@/components/dogs/DogForm';
import { fillDogForm } from '@/test/helpers/dogs/fillDogForm';

describe('DogForm validation UX', () => {
  const defaultCommand = {
    run: vi.fn(),
    errors: {},
    isSubmitting: false,
  };

  it('marks the name input as aria-invalid when it has an error', async () => {
    const user = userEvent.setup();

    render(
      <DogForm
        title="Register Dog"
        submitLabel="Register Dog"
        command={defaultCommand}
      />
    );

    await user.click(screen.getByRole('button', { name: /register dog/i }));

    await waitFor(() => {
      expect(screen.getByLabelText(/name/i)).toHaveAttribute('aria-invalid', 'true');
    });
  });

  it('links the name input to its error via aria-describedby', async () => {
    const user = userEvent.setup();

    render(
      <DogForm
        title="Register Dog"
        submitLabel="Register Dog"
        command={defaultCommand}
      />
    );

    await user.click(screen.getByRole('button', { name: /register dog/i }));

    await waitFor(() => {
      const input = screen.getByLabelText(/name/i);
      const errorId = input.getAttribute('aria-describedby');

      expect(errorId).toBeTruthy();
      expect(document.getElementById(errorId!)).toHaveTextContent(/please enter your dog's name/i);
    });
  });

  it('uses invitational tone for validation messages', async () => {
    const user = userEvent.setup();

    render(
      <DogForm
        title="Register Dog"
        submitLabel="Register Dog"
        command={defaultCommand}
      />
    );

    await user.click(screen.getByRole('button', { name: /register dog/i }));

    expect(screen.getByText("Please enter your dog's name")).toBeInTheDocument();
    expect(screen.getByText('Please enter a breed')).toBeInTheDocument();
    expect(screen.getByText('Please enter a date of birth')).toBeInTheDocument();
    expect(screen.getByText('Please select a sex')).toBeInTheDocument();
  });

  it('renders field errors with role="alert"', async () => {
    const user = userEvent.setup();

    render(
      <DogForm
        title="Register Dog"
        submitLabel="Register Dog"
        command={defaultCommand}
      />
    );

    await user.click(screen.getByRole('button', { name: /register dog/i }));

    await waitFor(() => {
      const alerts = screen.getAllByRole('alert');
      expect(alerts.length).toBeGreaterThanOrEqual(4);
    });
  });

  it('clears aria-invalid when fields are corrected and resubmitted', async () => {
    const user = userEvent.setup();

    render(
      <DogForm
        title="Register Dog"
        submitLabel="Register Dog"
        command={defaultCommand}
      />
    );

    await user.click(screen.getByRole('button', { name: /register dog/i }));

    expect(screen.getByLabelText(/name/i)).toHaveAttribute('aria-invalid', 'true');

    await fillDogForm(user);

    await user.click(screen.getByRole('button', { name: /register dog/i }));

    expect(screen.getByLabelText(/name/i)).not.toHaveAttribute('aria-invalid');
  });
});
