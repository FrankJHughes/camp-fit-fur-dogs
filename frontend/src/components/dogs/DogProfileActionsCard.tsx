import type { DogProfileAction } from '@/lib/dogs/dogProfileActions';

interface DogProfileActionsCardProps {
    actions: DogProfileAction[];
}

export default function DogProfileActionsCard({ actions }: DogProfileActionsCardProps) {
    return (
        <>
            {actions.map((action) => (
                <button key={action.label} onClick={action.onClick}>
                    {action.label}
                </button>
            ))}
        </>
    );
}
