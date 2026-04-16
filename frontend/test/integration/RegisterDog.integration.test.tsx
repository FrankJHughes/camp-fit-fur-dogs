import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import RegisterDogPage from '@/app/dogs/register/page';

const pushMock = vi.fn();

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

async function fillAndSubmitForm(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText(/name/i), 'Buddy');
  await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
  await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
  await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
  await user.click(screen.getByRole('button', { name: /register/i }));
}

describe('Register Dog (integration)', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  it('submits through the real API client and navigates on success', async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ success: true }),
    });
    vi.stubGlobal('fetch', fetchMock);

    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    await waitFor(() => {
      expect(pushMock).toHaveBeenCalledWith('/dogs/register/success');
    });

    expect(fetchMock).toHaveBeenCalledWith('/api/dogs/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        name: 'Buddy',
        breed: 'Golden Retriever',
        dateOfBirth: '2023-06-15',
        sex: 'Male',
      }),
    });
  });

  it('displays server validation errors without navigating', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
      ok: false,
      json: () => Promise.resolve({
        errors: { name: 'Name is already taken' },
      }),
    }));

    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    expect(await screen.findByText('Name is already taken')).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  });

  it('displays a network error when the server is unreachable', async () => {
    vi.stubGlobal('fetch', vi.fn().mockRejectedValue(new TypeError('Failed to fetch')));

    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    expect(
      await screen.findByText('A network error occurred. Please try again.')
    ).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  });
});
