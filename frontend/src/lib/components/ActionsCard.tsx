import type { Action } from '@/lib/action';

interface ActionsCardProps {
  header?: React.ReactNode;
  actions: Action[];
}

export function ActionsCard({ header, actions }: ActionsCardProps) {
  return (
    <div className="actions-card">
      <div className="actions-card-header" tabIndex={0}>
        {header}
      </div>

      <div className="actions-card-menu">
        {actions.map((action) => (
          <button
            key={action.label}
            onClick={action.onClick}
            disabled={action.disabled}
            data-variant={action.variant}
          >
            {action.label}
          </button>
        ))}
      </div>
    </div>
  );
}
