import { render, screen } from '@testing-library/react';

const { mockListDogs } = vi.hoisted(() => ({
  mockListDogs: vi.fn(),
}));

vi.mock('@/api/dogs/listDogs', () => ({
  listDogs: mockListDogs,
}));

vi.mock('next/link', () => ({
  default: (props: any) => <a href={props.href}>{props.children}</a>,
}));

import DogsPage from '@/app/dogs/page';

describe('DogsPage', () => {
  afterEach(() => {
    mockListDogs.mockReset();
  });

  it('shows loading state initially', () => {
    mockListDogs.mockReturnValue(new Promise(() => {}));
    render(<DogsPage />);
    expect(screen.getByText('Loading\u2026')).toBeInTheDocument();
  });

  it('renders list of dogs on success', async () => {
    mockListDogs.mockResolvedValue({
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
    mockListDogs.mockResolvedValue({
      success: true,
      data: { dogs: [] },
    });

    render(<DogsPage />);

    expect(
      await screen.findByText('No dogs registered yet.')
    ).toBeInTheDocument();
  });

  it('shows error message on failure', async () => {
    mockListDogs.mockResolvedValue({
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
