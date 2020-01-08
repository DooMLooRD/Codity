import React from 'react';
import {
  CardHeader,
  CardContent,
  Typography,
  IconButton,
  Menu,
  MenuItem,
} from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';

import { withLocalize, LocalizeContextProps } from 'react-localize-redux';
import { UserAvatar } from '../containers/profile/index';

interface Props extends LocalizeContextProps {
  authorFirstName: string;
  authorLastName: string;
  authorImage: string;
  commentDate: string;
  commentText: string;
}

const SingleComment: React.FC<Props> = (props: Props) => {
  const date = new Date(props.commentDate);
  const langCode = props.activeLanguage ? props.activeLanguage.code : 'en';
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <CardHeader
        avatar={
          <UserAvatar
            firstName={props.authorFirstName}
            lastName={props.authorLastName}
            photo={props.authorImage}
          />
        }
        action={
          <div>
            <IconButton onClick={handleMenuClick} aria-label="more">
              <MoreVertIcon />
            </IconButton>
            <Menu
              id="simple-menu"
              anchorEl={anchorEl}
              keepMounted
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
            >
              <MenuItem onClick={handleMenuClose}>Show User</MenuItem>

              <MenuItem onClick={handleMenuClose}>Delete comment</MenuItem>
              <MenuItem onClick={handleMenuClose}>Edit comment</MenuItem>
            </Menu>
          </div>
        }
        title={props.authorFirstName + ' ' + props.authorLastName}
        subheader={new Intl.DateTimeFormat(langCode, {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: '2-digit',
          hour: 'numeric',
          minute: 'numeric',
        }).format(date)}
      />

      <CardContent>
        <Typography variant="body2" color="textPrimary" component="p">
          {props.commentText}
        </Typography>
      </CardContent>
    </>
  );
};

export default withLocalize(SingleComment);
