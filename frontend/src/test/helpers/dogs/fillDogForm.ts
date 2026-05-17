import { screen, waitFor } from '@testing-library/react';
import type { UserEvent } from '@testing-library/user-event';

export async function fillDogForm(user: UserEvent) {
  await user.type(screen.getByLabelText(/name/i), 'Buddy');
  await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
  await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
  await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');

  // Ensure React has flushed controlled input updates
  await waitFor(() => {
    expect(screen.getByRole('button', { name: /register/i })).not.toBeDisabled();
  });
}
