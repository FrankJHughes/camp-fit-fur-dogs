import { removeDog } from '@/api/dogs/removeDog';

const { mockDelete } = vi.hoisted(() => ({ mockDelete: vi.fn() }));

vi.mock('@/lib/api/client', () => ({
    createApiClient: () => ({
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        delete: mockDelete,
    }),
}));

const dogId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';

describe('removeDog', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('returns success when the API call succeeds', async () => {
        mockDelete.mockResolvedValue({ ok: true, data: undefined });

        const result = await removeDog(dogId);

        expect(mockDelete).toHaveBeenCalledWith(`/dogs/${dogId}`);
        expect(result).toEqual({ success: true });
    });

    it('returns a form-level error on HTTP errors', async () => {
        mockDelete.mockResolvedValue({
            ok: false,
            error: {
                type: 'http',
                message: 'Internal Server Error',
                status: 500,
            },
        });

        const result = await removeDog(dogId);

        expect(result).toEqual({
            success: false,
            errors: { form: 'Internal Server Error' },
        });
    });

    it('returns a form-level error on network errors', async () => {
        mockDelete.mockResolvedValue({
            ok: false,
            error: {
                type: 'network',
                message: 'Failed to fetch',
            },
        });

        const result = await removeDog(dogId);

        expect(result).toEqual({
            success: false,
            errors: { form: 'A network error occurred. Please try again.' },
        });
    });
});
