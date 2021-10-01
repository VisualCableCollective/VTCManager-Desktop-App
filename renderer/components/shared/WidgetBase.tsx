interface Props {
  children: React.ReactNode;
  className?: string;
}

export function WidgetBase(props: Props) {
  return (
    <div className={"widget bg-dark-2 rounded box-border " + props.className}>
      {props.children}
    </div>
  )
}