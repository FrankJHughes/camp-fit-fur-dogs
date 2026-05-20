import { screen } from '@testing-library/react';
import type { UserEvent } from '@testing-library/user-event';

export async function fillCreateAccountForm(user: UserEvent) {
  await user.type(screen.getByLabelText(/first name/i), 'Frank');
  await user.type(screen.getByLabelText(/last name/i), 'Hughes');
  await user.type(screen.getByLabelText(/email/i), 'frank@example.com');

  // Must contain at least 10 digits (backend + frontend contract)
  await user.type(screen.getByLabelText(/phone/i), '916-555-1234');

  await user.type(screen.getByLabelText(/^password$/i), 'Password123!');
  await user.type(
    screen.getByLabelText(/confirm password/i),
    'Password123!'
  );
}
