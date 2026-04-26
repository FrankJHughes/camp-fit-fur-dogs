export interface DogProfileAction {
    label: string;
    onClick: () => void;
}

export function getDogProfileActions(
    id: string,
    push: (url: string) => void
): DogProfileAction[] {
    return [
        { label: 'Edit', onClick: () => push(`/dogs/${id}/edit`) },
    ];
}
