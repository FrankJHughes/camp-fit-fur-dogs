import type { Action } from '@/lib/shared/action';

interface ActionsCardProps {
    actions: Action[];
}

export function ActionsCard({ actions }: ActionsCardProps) {
    if (actions.length === 0) return null;

    return (
        <>
            {actions.map((action) => (
                <button
                    key={action.label}
                    onClick={action.onClick}
                    {...(action.variant && action.variant !== 'default'
                        ? { 'data-variant': action.variant }
                        : {})}
                >
                    {action.label}
                </button>
            ))}
        </>
    );
}
