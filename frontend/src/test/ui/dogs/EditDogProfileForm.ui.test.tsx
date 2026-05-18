import { describe, it, expect, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import {
  useApiCommandMock,
  setUseApiCommandReturn,
  resetUseApiCommandMock,
} from '@/test/helpers/mockUseApiCommand';

describe('EditDogProfileForm (UI)', () => {
  beforeEach(() => {
    resetUseApiCommandMock();
  });

  async function loadForm() {
    const mod = await import('@/components/dogs/EditDogProfileForm');
    return mod.EditDogProfileForm || mod.default;
  }

  const initialData = {
    name: 'Rex',
    breed: 'Labrador',
    dateOfBirth: '2020-01-01',
    sex: 'Male',
  };

  it('renders all fields with initial values', async () => {
    setUseApiCommandReturn({
      errors: {},
      isSubmitting: false,
      handleSubmit: vi.fn(),
    });

    const EditDogProfileForm = await loadForm();

    render(
      <EditDogProfileForm
        initialData={initialData}
        onSubmit={vi.fn()}
        errors={{}}
        isSubmitting={false}
      />
    );

    expect(screen.getByLabelText(/name/i)).toHaveValue('Rex');
    expect(screen.getByLabelText(/breed/i)).toHaveValue('Labrador');
    expect(screen.getByLabelText(/date of birth/i)).toHaveValue('2020-01-01');
    expect(screen.getByLabelText(/sex/i)).toHaveValue('Male');
  });

  it('submits edited values', async () => {
    const user = userEvent.setup();
    const handleSubmit = vi.fn();

    setUseApiCommandReturn({
      errors: {},
      isSubmitting: false,
      handleSubmit,
    });

    const EditDogProfileForm = await loadForm();

    render(
      <EditDogProfileForm
        initialData={initialData}
        onSubmit={handleSubmit}
        errors={{}}
        isSubmitting={false}
      />
    );

    await user.clear(screen.getByLabelText(/name/i));
    await user.type(screen.getByLabelText(/name/i), 'Buddy');

    await user.clear(screen.getByLabelText(/breed/i));
    await user.type(screen.getByLabelText(/breed/i), 'Husky');

    await user.clear(screen.getByLabelText(/date of birth/i));
    await user.type(screen.getByLabelText(/date of birth/i), '2019-05-10');

    await user.selectOptions(screen.getByLabelText(/sex/i), 'Female');

    await user.click(screen.getByRole('button', { name: /save/i }));

    expect(handleSubmit).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Husky',
      dateOfBirth: '2019-05-10',
      sex: 'Female',
    });
  });

  it('renders field-level errors', async () => {
    setUseApiCommandReturn({
      errors: {
        name: 'Name is required',
        breed: 'Breed is required',
      },
      isSubmitting: false,
      handleSubmit: vi.fn(),
    });

    const EditDogProfileForm = await loadForm();

    render(
      <EditDogProfileForm
        initialData={initialData}
        onSubmit={vi.fn()}
        errors={{
          name: 'Name is required',
          breed: 'Breed is required',
        }}
        isSubmitting={false}
      />
    );

    expect(screen.getByText(/name is required/i)).toBeInTheDocument();
    expect(screen.getByText(/breed is required/i)).toBeInTheDocument();
  });

  it('disables submit button while submitting', async () => {
    setUseApiCommandReturn({
      errors: {},
      isSubmitting: true,
      handleSubmit: vi.fn(),
    });

    const EditDogProfileForm = await loadForm();

    render(
      <EditDogProfileForm
        initialData={initialData}
        onSubmit={vi.fn()}
        errors={{}}
        isSubmitting={true}
      />
    );

    expect(screen.getByRole('button', { name: /save/i })).toBeDisabled();
  });
});
