import React from 'react';
import { Avatar, Button } from '@material-ui/core';
import AddAPhotoIcon from '@material-ui/icons/AddAPhoto';
import { toast } from 'react-toastify';
import {
  withLocalize,
  LocalizeContextProps,
  Translate as T,
} from 'react-localize-redux';

import { profileTranslations } from '../../../translations/index';

interface Props extends LocalizeContextProps {
  className?: string;
  handleImage: (base64Image: string) => void;
  existingPic?: string;
}

const AddPhotoAvatar: React.FC<Props> = (props: Props) => {
  props.addTranslation(profileTranslations);

  const [loadedImg, setLoadedImg] = React.useState<string | ArrayBuffer | null>(
    props.existingPic ? props.existingPic : '',
  );
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const images = e.target.files;

    if (images !== null && images.length > 0) {
      const image = images[0];
      const reader = new FileReader();

      reader.readAsDataURL(image);
      reader.onload = () => {
        if (!image.type.includes('image/')) {
          toast.error(<T id="imageOfType" />);
        } else if (Math.round(image.size / 1000) <= 1024) {
          setLoadedImg(reader.result);
          props.handleImage(reader.result as string);
        } else {
          toast.error(<T id="imageOfSize" />);
        }
      };
    }
  };

  const handleRemovePicButton = () => {
    setLoadedImg('');
    props.handleImage('');
  };
  return (
    <div>
      <input
        accept="image/*"
        style={{ display: 'none' }}
        id="avatar-input-file"
        multiple={false}
        type="file"
        onChange={handleChange}
      />
      <label htmlFor="avatar-input-file">
        <Avatar
          style={{ cursor: 'pointer ' }}
          className={props.className || ''}
          aria-label="person"
          src={typeof loadedImg === 'string' ? loadedImg : ''}
        >
          <AddAPhotoIcon />
        </Avatar>
        <Button
          onClick={handleRemovePicButton}
          variant="outlined"
          color="secondary"
          style={{ margin: '8px' }}
        >
          <T id="removePicture" />
        </Button>
      </label>
    </div>
  );
};

export default withLocalize(AddPhotoAvatar);
