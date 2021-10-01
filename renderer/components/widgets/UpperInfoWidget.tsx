import { WidgetBase } from "../shared/WidgetBase";

// icons
import { FaGasPump } from "react-icons/fa";
import { WiDayRain } from "react-icons/wi";

export function UpperInfoWidget() {
  return (
    <WidgetBase className="p-5">
      <div className="grid grid-cols-3 gap-2 lg:gap-28">
        <div className="flex items-center">
          <div className="text-xl mr-2">
            <FaGasPump />
          </div>
          <WidgetDefaultText>85%</WidgetDefaultText>
        </div>
        <div className="flex justify-between">
          <div className="flex items-center">
            <div className="text-2xl mr-2">
              <WiDayRain />
            </div>
            <WidgetDefaultText>Raining</WidgetDefaultText>
          </div>
          <WidgetDefaultText>22 C</WidgetDefaultText>
        </div>
        <div className="flex justify-between">
          <WidgetDefaultText>Sat 28 Aug 2021</WidgetDefaultText>
          <WidgetDefaultText>16:28</WidgetDefaultText>
        </div>
      </div>
    </WidgetBase>
  );
}

// ToDo: move this
interface WDTProps {
  children: React.ReactNode;
}
function WidgetDefaultText(props: WDTProps) {
  return <h1 className="text-opacity-80 text-white">{props.children}</h1>;
}
