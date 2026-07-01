'use client';

import { ActionsCard } from '@/lib/components/ActionsCard';

export default function LoggedOutPage() {
  return (
    <div className="logged-out-page">
      <h2>You’ve been logged out</h2>
      <p>Your session has ended. You can log back in at any time.</p>

      <ActionsCard
        actions={[
          {
            label: 'Return to Login',
            variant: 'default',
            onClick: () => {
              window.location.href = '/login';
            },
          },
        ]}
      />
    </div>
  );
}
