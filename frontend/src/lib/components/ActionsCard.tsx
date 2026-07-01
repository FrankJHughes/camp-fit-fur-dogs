import type { Action } from '@/lib/action';

interface ActionsCardProps {
  actions: Action[];
  defaultVariant?: Action['variant'];
}

export function ActionsCard({ actions }: ActionsCardProps) {
  if (actions.length === 0) return null;

  return (
    <>
      {actions.map((action) => (
        <button
          key={action.label}
          onClick={action.onClick}
          disabled={action.disabled}
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
