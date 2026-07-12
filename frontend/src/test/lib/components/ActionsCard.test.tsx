import { describe, it, expect, vi } from 'vitest';
import { render } from '@testing-library/react';
import { ActionsCard } from '@/lib/components/ActionsCard';

describe('ActionsCard', () => {
  it('renders empty card structure when actions array is empty', () => {
    const { container } = render(
      <ActionsCard header="Menu" actions={[]} />
    );

    const card = container.querySelector('.actions-card');
    expect(card).toBeInTheDocument();

    const menu = container.querySelector('.actions-card-menu');
    expect(menu).toBeInTheDocument();
    expect(menu?.children.length).toBe(0);
  });

  it('renders actions as buttons when provided', () => {
    const onClick = vi.fn();

    const { getByRole } = render(
      <ActionsCard
        header="Menu"
        actions={[
          { label: 'Login with Auth0', variant: 'primary', onClick },
        ]}
      />
    );

    const button = getByRole('button', { name: /login with auth0/i });
    expect(button).toBeInTheDocument();

    button.click();
    expect(onClick).toHaveBeenCalled();
  });
});
