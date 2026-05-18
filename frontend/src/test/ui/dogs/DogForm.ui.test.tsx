import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

describe('DogForm (UI)', () => {
  async function loadForm() {
    const mod = await import('@/components/dogs/DogForm');
    return mod.DogForm || mod.default;
  }

  const defaultProps = {
    title: 'Register Dog',
    submitLabel: 'Register Dog',
    onSubmit: vi.fn(),
    initialValues: {
      name: '',
      breed: '',
      dateOfBirth: '',
      sex: 'unknown',
    },
    errors: {},
    isSubmitting: false,
  };

  it('renders all fields', async () => {
    const DogForm = await loadForm();
    render(<DogForm {...defaultProps} />);

    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/breed/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/date of birth/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/sex/i)).toBeInTheDocument();
  });

  it('calls onSubmit with form values', async () => {
    const DogForm = await loadForm();
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(<DogForm {...defaultProps} onSubmit={onSubmit} />);

    await user.type(screen.getByLabelText(/name/i), 'Rex');
    await user.type(screen.getByLabelText(/breed/i), 'Labrador');
    await user.type(screen.getByLabelText(/date of birth/i), '2020-01-01');
    await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');

    await user.click(
      screen.getByRole('button', { name: /register dog/i }),
    );

    expect(onSubmit).toHaveBeenCalledWith({
      name: 'Rex',
      breed: 'Labrador',
      dateOfBirth: '2020-01-01',
      sex: 'Male',
    });
  });
});
