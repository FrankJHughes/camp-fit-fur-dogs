import { screen } from '@testing-library/react';
import type { UserEvent } from '@testing-library/user-event';

export async function fillCreateAccountForm(user: UserEvent) {
  await user.type(screen.getByLabelText(/email/i), 'frank@example.com');
  await user.type(screen.getByLabelText(/^password$/i), 'Password123!');
  await user.type(screen.getByLabelText(/confirm password/i), 'Password123!');
}
