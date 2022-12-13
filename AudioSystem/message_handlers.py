"""! Handlers for various messages"""
import logging


def handle_message(topic, payload):
    """! Main entry point to the module """
    if topic == "play":
        play_audio(payload)
    if topic == "config":
        handle_config(payload)

def play_audio(payload):
    """! Handle the queueing up of audio """
    from audio import audio                   # pylint: disable=import-outside-toplevel
    try:
        platform = payload['platform']
        audio_type = payload['audio']
    except KeyError:
        # Invalid message
        logging.debug("Invalid message: %s", payload)
        return
    left = False
    right = False
    if platform == 0:
        # Play on both platforms
        left = True
        right = True
    elif platform == 1:
        # Just platform 1
        left = True
    elif platform == 2:
        # Just platform 2
        right = True
    else:
        # Invalid platform selection
        return
    audio.queue_audio(audio_type, left, right)

def handle_config(payload):
    """! Handle the changing of config"""
