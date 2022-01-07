import React from "react";
import { VehicleWidget } from "../components/widgets/VehicleWidget";
import { CenterWidgetContainer } from "../components/widgets/CenterWidgetContainer";

function Dashboard() {
  return (
    <div className="w-full h-full bg-dark-1 flex p-6 select-none">
      <div className="w-2/3">
        <CenterWidgetContainer />
      </div>
      <VehicleWidget />
    </div>
  );
}

export default Dashboard;
