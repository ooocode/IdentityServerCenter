

import * as React from 'react';
import { Toggle } from 'office-ui-fabric-react/lib/Toggle';
import { Layer, LayerHost } from 'office-ui-fabric-react/lib/Layer';
import { AnimationClassNames, mergeStyleSets, getTheme } from 'office-ui-fabric-react/lib/Styling';
import { IToggleStyles } from 'office-ui-fabric-react/lib/Toggle';
import { useId, useBoolean } from '@uifabric/react-hooks';
import { Link } from 'gatsby';
import { Stack } from '@fluentui/react'
import { Nav, INavLink, INavStyles, INavLinkGroup } from 'office-ui-fabric-react/lib/Nav';

const navStyles: Partial<INavStyles> = {
    root: {
        width: 208,
        height: 350,
        boxSizing: 'border-box',
        border: '1px solid #eee',
        overflowY: 'auto',
    },
};

const navLinkGroups: INavLinkGroup[] = [
    {
        links: [
            {
                name: 'Home',
                url: 'http://example.com',
                expandAriaLabel: 'Expand Home section',
                collapseAriaLabel: 'Collapse Home section',
                links: [
                    {
                        name: 'Activity',
                        url: 'http://msn.com',
                        key: 'key1',
                        target: '_blank',
                    },
                    {
                        name: 'MSN',
                        url: 'http://msn.com',
                        disabled: true,
                        key: 'key2',
                        target: '_blank',
                    },
                ],
                isExpanded: true,
            },
            {
                name: 'Documents',
                url: 'http://example.com',
                key: 'key3',
                isExpanded: true,
                target: '_blank',
            },
            {
                name: 'Pages',
                url: 'http://msn.com',
                key: 'key4',
                target: '_blank',
            },
            {
                name: 'Notebook',
                url: 'http://msn.com',
                key: 'key5',
                disabled: true,
            },
            {
                name: 'Communication and Media',
                url: 'http://msn.com',
                key: 'key6',
                target: '_blank',
            },
            {
                name: 'News',
                url: 'http://cnn.com',
                icon: 'News',
                key: 'key7',
                target: '_blank',
            },
        ],
    },
];
const NavBasicExample: React.FunctionComponent = () => {
    return (
        <Nav
            onLinkClick={_onLinkClick}
            selectedKey="key3"
            ariaLabel="Nav basic example"
            styles={navStyles}
            groups={navLinkGroups}
        />
    );
};

function _onLinkClick(ev?: React.MouseEvent<HTMLElement>, item?: INavLink) {
    if (item && item.name === 'News') {
        alert('News link clicked');
    }
}


export const Layout: React.FunctionComponent = ({ children }) => {
    const [showLayerNoId, { toggle: toggleShowLayerNoId }] = useBoolean(true);

    const content = <div className={styles.content}>
        <span>学习坊后台管理</span>
    </div>;

    const style = {
        marginTop: 55
    };

    return (
        <div className={styles.root}>
            {showLayerNoId ? (
                <Layer onLayerDidMount={logDidMount} onLayerWillUnmount={logWillUnmount}>
                    {content}
                </Layer>
            ) : (
                    content
                )}
            <div style={style}>
                <Stack>
                    <Stack>
                        <NavBasicExample />
                    </Stack>

                    <Stack>
                        {children}
                    </Stack>


                </Stack>

            </div>
        </div>
    );
};

const logDidMount = () => console.log('layer did mount');
const logWillUnmount = () => console.log('layer will unmount');

const toggleStyles: Partial<IToggleStyles> = {
    root: { margin: '10px 0' },
};
const theme = getTheme();
const styles = {
    toggle: toggleStyles,
    ...mergeStyleSets({
        root: {
            selectors: { p: { marginTop: 30 } },
        },

        customHost: {
            height: 100,
            padding: 20,
            background: 'rgba(255, 0, 0, 0.2)',
            border: '1px dashed ' + theme.palette.black,
            position: 'relative',
            selectors: {
                '&:before': {
                    position: 'absolute',
                    left: '50%',
                    top: '50%',
                    transform: 'translate(-50%, -50%)',
                    // double quotes required to make the string show up
                    content: '"I am a LayerHost with id=layerhost1"',
                },
            },
        },
        content: [
            {
                backgroundColor: theme.palette.themePrimary,
                color: theme.palette.white,
                lineHeight: '50px',
                padding: '0 20px',
            },
            AnimationClassNames.scaleUpIn100,
        ],
        nonLayered: {
            backgroundColor: theme.palette.neutralTertiaryAlt,
            lineHeight: '50px',
            padding: '0 20px',
            margin: '8px 0',
        },
    }),
};
