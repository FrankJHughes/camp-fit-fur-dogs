import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeAll } from 'vitest';
import { ConfirmDialog } from '../../../src/components/shared/ConfirmDialog';

beforeAll(() => {
  HTMLDialogElement.prototype.showModal ??= vi.fn(function (this: HTMLDialogElement) {
    this.setAttribute('open', '');
  });
  HTMLDialogElement.prototype.close ??= vi.fn(function (this: HTMLDialogElement) {
    this.removeAttribute('open');
  });
});

describe('ConfirmDialog', () => {
  const baseProps = {
    isOpen: true,
    title: 'Remove Buddy?',
    description:
      'This will permanently remove Buddy from your account. This cannot be undone.',
    confirmLabel: 'Remove',
    onConfirm: vi.fn(),
    onCancel: vi.fn(),
  };

  it('renders dialog content when open', () => {
    render(<ConfirmDialog {...baseProps} />);
    expect(screen.getByRole('dialog')).toBeInTheDocument();
  });

  it('does not expose dialog role when closed', () => {
    render(<ConfirmDialog {...baseProps} isOpen={false} />);
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
  });

  it('displays the title describing the action', () => {
    render(<ConfirmDialog {...baseProps} />);
    expect(
      screen.getByRole('heading', { name: 'Remove Buddy?' }),
    ).toBeInTheDocument();
  });

  it('displays the description with consequences', () => {
    render(<ConfirmDialog {...baseProps} />);
    expect(
      screen.getByText(/permanently remove Buddy/),
    ).toBeInTheDocument();
  });

  it('calls onConfirm when confirm button is clicked', async () => {
    const onConfirm = vi.fn();
    render(<ConfirmDialog {...baseProps} onConfirm={onConfirm} />);
    await userEvent.click(screen.getByRole('button', { name: 'Remove' }));
    expect(onConfirm).toHaveBeenCalledOnce();
  });

  it('calls onCancel when cancel button is clicked', async () => {
    const onCancel = vi.fn();
    render(<ConfirmDialog {...baseProps} onCancel={onCancel} />);
    await userEvent.click(screen.getByRole('button', { name: 'Cancel' }));
    expect(onCancel).toHaveBeenCalledOnce();
  });

  it('focuses the cancel button by default, not confirm', () => {
    render(<ConfirmDialog {...baseProps} />);
    expect(screen.getByRole('button', { name: 'Cancel' })).toHaveFocus();
  });

  it('calls onCancel when Escape is pressed', async () => {
    const onCancel = vi.fn();
    render(<ConfirmDialog {...baseProps} onCancel={onCancel} />);
    await userEvent.keyboard('{Escape}');
    expect(onCancel).toHaveBeenCalledOnce();
  });

  it('keeps the dialog element in the DOM when isOpen is false', () => {
    const { container } = render(<ConfirmDialog {...baseProps} isOpen={false} />);

    expect(container.querySelector('dialog')).toBeInTheDocument();
  });

  it('removes the open attribute when isOpen transitions to false', () => {
    const { container, rerender } = render(<ConfirmDialog {...baseProps} isOpen={true} />);
    expect(container.querySelector('dialog')).toHaveAttribute('open');

    rerender(<ConfirmDialog {...baseProps} isOpen={false} />);
    expect(container.querySelector('dialog')).toBeInTheDocument();
    expect(container.querySelector('dialog')).not.toHaveAttribute('open');
  });
});
