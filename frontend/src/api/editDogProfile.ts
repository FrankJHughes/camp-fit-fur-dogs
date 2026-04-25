export interface EditDogProfileData {
    name: string;
    breed: string;
    dateOfBirth: string;
    sex: string;
}

export interface EditDogProfileResult {
    success: boolean;
    errors?: Record<string, string>;
}

export async function editDogProfile(
    dogId: string,
    data: EditDogProfileData
): Promise<EditDogProfileResult> {
    try {
        const response = await fetch(`/api/dogs/${dogId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            try {
                const body = await response.json();
                return { success: false, errors: body.errors };
            } catch {
                return {
                    success: false,
                    errors: {
                        form: 'An unexpected error occurred. Please try again.',
                    },
                };
            }
        }

        // 204 No Content — no body to parse
        return { success: true };
    } catch {
        return {
            success: false,
            errors: {
                form: 'A network error occurred. Please try again.',
            },
        };
    }
}
