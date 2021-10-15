import React, {createContext, useContext, useEffect, useState} from "react";
import electron, {IpcRenderer} from 'electron';

interface ContextProps {
    ipcRenderer: IpcRenderer
}

const IpcContext = createContext<ContextProps>({
    ipcRenderer: null
});

interface Props {
    children: React.ReactNode,
}
export function IpcContextProvider(props: Props) {
    const [ipcRenderer, setIpcRenderer] = useState<IpcRenderer>(null);

    if(!ipcRenderer) {
        const foundIpcRenderer = electron.ipcRenderer || false;
        if (foundIpcRenderer) {
            setIpcRenderer(foundIpcRenderer);
            foundIpcRenderer.on("telemetry", (event, data) => {
                console.log(event);
                console.log(data);
            });
        }
    }

    const context = {
        ipcRenderer: ipcRenderer,
    };

    return (
        <IpcContext.Provider value={context}>
            {props.children}
        </IpcContext.Provider>
    )
}

export const useIpc = () => useContext(IpcContext);
