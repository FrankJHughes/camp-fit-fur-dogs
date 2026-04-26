import { render, screen } from '@testing-library/react';
import { FieldError } from '../../../src/components/shared/FieldError';

describe('FieldError', () => {
  it('renders the error message with role="alert"', () => {
    render(<FieldError id="error-name" message="Please enter your dog's name" />);
    expect(screen.getByRole('alert')).toHaveTextContent(
      "Please enter your dog's name",
    );
  });

  it('renders with the provided id for aria-describedby linkage', () => {
    render(<FieldError id="error-name" message="Please enter your dog's name" />);
    expect(screen.getByRole('alert')).toHaveAttribute('id', 'error-name');
  });

  it('does not render when message is undefined', () => {
    const { container } = render(
      <FieldError id="error-name" message={undefined} />,
    );
    expect(container).toBeEmptyDOMElement();
  });
});