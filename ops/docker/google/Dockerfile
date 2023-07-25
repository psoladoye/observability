FROM gcr.io/google.com/cloudsdktool/google-cloud-cli:alpine
RUN apk --update add openjdk8-jre
RUN gcloud components install --quiet beta pubsub-emulator
CMD gcloud beta emulators pubsub start --project=${PROJECT_ID}