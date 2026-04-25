import { editDogProfile } from '@/api/editDogProfile';

const dogId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';

const validData = {
    name: 'Buddy',
    breed: 'Golden Retriever',
    dateOfBirth: '2023-06-15',
    sex: 'Male',
};

describe('editDogProfile', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('sends a PUT request to /api/dogs/{dogId}', async () => {
        const fetchMock = vi.fn().mockResolvedValue({
            ok: true,
            status: 204,
        });
        global.fetch = fetchMock;

        const result = await editDogProfile(dogId, validData);

        expect(fetchMock).toHaveBeenCalledWith(`/api/dogs/${dogId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(validData),
        });
        expect(result).toEqual({ success: true });
    });

    it('returns failure with errors when the response is not ok', async () => {
        global.fetch = vi.fn().mockResolvedValue({
            ok: false,
            json: () =>
                Promise.resolve({
                    errors: { name: 'Name is required' },
                }),
        });

        const result = await editDogProfile(dogId, validData);

        expect(result).toEqual({
            success: false,
            errors: { name: 'Name is required' },
        });
    });

    it('returns failure with a network error when fetch throws', async () => {
        global.fetch = vi
            .fn()
            .mockRejectedValue(new TypeError('Failed to fetch'));

        const result = await editDogProfile(dogId, validData);

        expect(result).toEqual({
            success: false,
            errors: { form: 'A network error occurred. Please try again.' },
        });
    });

    it('returns failure with a generic error when the error response is not JSON', async () => {
        global.fetch = vi.fn().mockResolvedValue({
            ok: false,
            json: () =>
                Promise.reject(new SyntaxError('Unexpected token <')),
        });

        const result = await editDogProfile(dogId, validData);

        expect(result).toEqual({
            success: false,
            errors: {
                form: 'An unexpected error occurred. Please try again.',
            },
        });
    });
});
