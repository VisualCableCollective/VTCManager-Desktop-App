import { WidgetBase } from "../shared/WidgetBase";

// icons
import { AiFillMessage } from "react-icons/ai";
import { IoMdSettings, IoMdDesktop, IoMdMap } from "react-icons/io";
import { UpperInfoWidget } from "./UpperInfoWidget";

export function CenterWidgetContainer() {
  return (
    <div className="grid grid-cols-2 gap-6 w-full h-full" style={{gridTemplateRows: 'auto 1fr'}}>
      <div className="col-span-2">
        <UpperInfoWidget />
      </div>
      <h1>Nav</h1>
      <div className="grid grid-cols-2 gap-6 h-full place-self-stretch">
        <Shortcut title="Web Dashboard" icon={<IoMdDesktop />} />
        <Shortcut title="Messages" icon={<AiFillMessage />} />
        <Shortcut title="Map" icon={<IoMdMap />} />
        <Shortcut title="Settings" icon={<IoMdSettings />} />
      </div>
    </div>
  );
}

interface SCProps {
  title: string;
  icon: React.ReactNode;
}
function Shortcut(props: SCProps) {
  return (
    <WidgetBase className="flex items-center justify-center widget-clickable hover:scale-105 transform transition-all duration-200">
      <div>
        <div className="text-4xl flex justify-center">{props.icon}</div>
        <h1 className="text-opacity-80 text-white text-center mt-2">
          {props.title}
        </h1>
      </div>
    </WidgetBase>
  );
}
