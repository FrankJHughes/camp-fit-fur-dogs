import { render, screen } from '@testing-library/react';

const { mockListDogsByCurrentUser } = vi.hoisted(() => ({
  mockListDogsByCurrentUser: vi.fn(),
}));

vi.mock('@/api/dogs/listDogsByCurrentUser', () => ({
  listDogsByCurrentUser: mockListDogsByCurrentUser,
}));

vi.mock('next/link', () => ({
  default: (props: { href: string; children: React.ReactNode }) => <a href={props.href}>{props.children}</a>,
}));

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: vi.fn() }),
}));

import DogsPage from '@/app/dogs/page';

describe('DogsPage', () => {
  afterEach(() => {
    mockListDogsByCurrentUser.mockReset();
  });

  it('shows loading state initially', () => {
    mockListDogsByCurrentUser.mockReturnValue(new Promise(() => { }));
    render(<DogsPage />);
    expect(screen.getByText('Loading…')).toBeInTheDocument();
  });

  it('renders list of dogs on success', async () => {
    mockListDogsByCurrentUser.mockResolvedValue({
      success: true,
      data: {
        dogs: [
          { id: '1', name: 'Biscuit', breed: 'Golden Retriever' },
          { id: '2', name: 'Maple', breed: 'Beagle' },
        ],
      },
    });

    render(<DogsPage />);

    expect(await screen.findByText(/Biscuit/)).toBeInTheDocument();
    expect(screen.getByText(/Maple/)).toBeInTheDocument();
  });

  it('shows empty state when no dogs registered', async () => {
    mockListDogsByCurrentUser.mockResolvedValue({
      success: true,
      data: { dogs: [] },
    });

    render(<DogsPage />);

    expect(
      await screen.findByText('No dogs registered yet.')
    ).toBeInTheDocument();
  });

  it('shows error message on failure', async () => {
    mockListDogsByCurrentUser.mockResolvedValue({
      success: false,
      notFound: false,
      error: 'Something went wrong',
    });

    render(<DogsPage />);

    expect(await screen.findByRole('alert')).toHaveTextContent(
      'Something went wrong'
    );
  });
});
