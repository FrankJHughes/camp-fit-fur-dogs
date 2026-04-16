import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import RegisterDogPage from '@/app/dogs/register/page';
import { registerDog } from '@/api/registerDog';

const pushMock = vi.fn();

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

vi.mock('@/api/registerDog', () => ({
  registerDog: vi.fn().mockResolvedValue({ success: true }),
}));

async function fillForm(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText(/name/i), 'Buddy');
  await user.type(screen.getByLabelText(/breed/i), 'Golden Retriever');
  await user.type(screen.getByLabelText(/date of birth/i), '2023-06-15');
  await user.selectOptions(screen.getByLabelText(/sex/i), 'Male');
}

async function fillAndSubmitForm(user: ReturnType<typeof userEvent.setup>) {
  await fillForm(user);
  await user.click(screen.getByRole('button', { name: /register/i }));
}

describe('Register Dog Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders the register dog form', () => {
    render(<RegisterDogPage />);

    expect(
      screen.getByRole('heading', { name: /register dog/i })
    ).toBeInTheDocument();
  });

  it('calls registerDog API when the form is submitted', async () => {
    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    expect(registerDog).toHaveBeenCalledWith({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
  }, 10000);

  it('navigates to success route after successful registration', async () => {
    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    expect(pushMock).toHaveBeenCalledWith('/dogs/register/success');
  }, 10000);

  it('displays validation errors returned by the API', async () => {
    vi.mocked(registerDog).mockResolvedValueOnce({
      success: false,
      errors: {
        name: 'Name is required',
        breed: 'Breed is required',
      },
    });

    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    expect(await screen.findByText('Name is required')).toBeInTheDocument();
    expect(screen.getByText('Breed is required')).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  }, 10000);

  it('disables the submit button while the API call is in flight', async () => {
    let resolveApi!: (value: { success: boolean }) => void;
    vi.mocked(registerDog).mockImplementationOnce(
      () => new Promise(resolve => { resolveApi = resolve; })
    );

    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillForm(user);
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByRole('button', { name: /register/i })).toBeDisabled();

    resolveApi({ success: true });

    await screen.findByRole('button', { name: /register/i });
  }, 10000);

  it('displays a form-level error when the API returns a network error', async () => {
    vi.mocked(registerDog).mockResolvedValueOnce({
      success: false,
      errors: { form: 'A network error occurred. Please try again.' },
    });

    const user = userEvent.setup();
    render(<RegisterDogPage />);

    await fillAndSubmitForm(user);

    expect(
      await screen.findByText('A network error occurred. Please try again.')
    ).toBeInTheDocument();
  }, 10000);
});
